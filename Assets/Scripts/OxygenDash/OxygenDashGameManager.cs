using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OxygenDashGameManager : MonoBehaviour
{
    public static OxygenDashGameManager Instance;

    [Header("Lives")]
    public int maxLives = 3;
    private int lives;

    [Header("Lives UI - Text (optional)")]
    public TMP_Text livesText;

    [Header("Lives UI - Hearts")]
    public Image[] lifeImages;       // order: left -> right
    public Sprite lifeFullSprite;
    public Sprite lifeEmptySprite;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    
    // add near your other headers
    [Header("Damage Feedback")]
    public ScreenFlash damageFlash;
    
    [Header("Audio (SFX)")]
    public AudioSource sfxSource;      // put an AudioSource on this object and drag it here (or leave empty to auto-find)
    public AudioClip damageClip;
    public AudioClip victoryClip;
    public AudioClip gameOverClip;

    [Range(0f, 0.2f)]
    public float pitchRandomness = 0.05f;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Time.timeScale = 1f;

        lives = maxLives;
        UpdateLivesUI(lives);

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);
    }

    // -------------------------------
    // PUBLIC: called when player takes damage
    // -------------------------------
    public void LoseLife()
    {
        if (gameEnded) return;

        lives--;
        UpdateLivesUI(lives);

        // ✅ sound + flash
        PlaySfx(damageClip);

        if (damageFlash != null)
            damageFlash.Flash();

        if (lives <= 0)
            GameOver();
    }

    // -------------------------------
    // PUBLIC: called when reaching the end cell
    // -------------------------------
    public void Victory()
    {
        if (gameEnded) return;
        gameEnded = true;

        PlaySfx(victoryClip);

        if (victoryPanel)
            victoryPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // -------------------------------
    void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        PlaySfx(gameOverClip);

        if (gameOverPanel)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // -------------------------------
    // HEART + TEXT UI UPDATE
    // -------------------------------
    private void UpdateLivesUI(int newLives)
    {
        // Clamp to display count if hearts exist, otherwise to maxLives
        int maxDisplay = (lifeImages != null && lifeImages.Length > 0) ? lifeImages.Length : maxLives;
        newLives = Mathf.Clamp(newLives, 0, maxDisplay);

        // optional text
        if (livesText)
            livesText.text = "Lives: " + newLives;

        // hearts
        if (lifeImages != null && lifeImages.Length > 0)
        {
            for (int i = 0; i < lifeImages.Length; i++)
            {
                Image img = lifeImages[i];
                if (img == null) continue;

                bool hasLife = i < newLives;

                if (hasLife)
                {
                    if (lifeFullSprite != null) img.sprite = lifeFullSprite;
                    img.enabled = true;
                }
                else
                {
                    if (lifeEmptySprite != null)
                    {
                        img.sprite = lifeEmptySprite;
                        img.enabled = true;
                    }
                    else
                    {
                        img.enabled = false;
                    }
                }
            }
        }

        lives = newLives;
    }

    public bool IsGameEnded() => gameEnded;

    // -------------------------------
    // Buttons
    // -------------------------------
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;

        if (sfxSource == null)
        {
            Debug.LogWarning("OxygenDashGameManager: No AudioSource assigned for SFX.");
            return;
        }

        sfxSource.pitch = 1f + Random.Range(-pitchRandomness, pitchRandomness);
        sfxSource.PlayOneShot(clip, 1f);
    }
}
