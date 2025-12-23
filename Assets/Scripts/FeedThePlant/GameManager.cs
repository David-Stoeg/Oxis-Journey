using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public Image scoreFill; // UI Image with Fill (Type = Filled)

    [Header("Lives UI")]
    public Image[] lifeImages;        // order: left -> right
    public Sprite lifeFullSprite;     // sprite for a life present
    public Sprite lifeEmptySprite;    // sprite for a life lost

    [Header("Panels")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public GameObject victoryPanel;
    public TMP_Text victoryScoreText;

    [Header("Game Settings")]
    public int maxLives = 3;
    public int requiredScore = 20;    // WIN CONDITION

    [Header("Audio (SFX)")]
    public AudioSource sfxSource;     // add an AudioSource on the same object and drag it here (or leave empty to auto-find)
    public AudioClip scoreClip;
    public AudioClip loseLifeClip;
    public AudioClip victoryClip;
    public AudioClip gameOverClip;

    [Range(0f, 0.2f)]
    public float pitchRandomness = 0.05f;

    private int _score = 0;
    private int _lives;
    private bool _isGameOver = false;
    private bool _isVictory = false;
    
    [Header("Damage Feedback")]
    public ScreenFlash damageFlash;

    // ---------- UNITY LIFECYCLE ----------

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // auto-find AudioSource if not assigned
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Time.timeScale = 1f;
        _lives = maxLives;

        UpdateScoreText();
        UpdateLives(_lives);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        if (_isGameOver || _isVictory) return;
    }

    // ---------- GAME FLOW ----------

    public void AddScore(int amount)
    {
        if (_isGameOver || _isVictory) return;

        _score += amount;
        UpdateScoreText();

        PlaySfx(scoreClip);

        if (_score >= requiredScore)
        {
            Victory();
        }
    }

    public void UpdateLives(int newLives)
    {
        int maxDisplay = (lifeImages != null && lifeImages.Length > 0) ? lifeImages.Length : maxLives;
        newLives = Mathf.Clamp(newLives, 0, maxDisplay);

        // ✅ Play lose-life sound whenever lives decrease (but not on Start)
        if (!_isGameOver && !_isVictory && newLives < _lives)
        {
            PlaySfx(loseLifeClip);
        }

        if (livesText != null)
            livesText.text = "Lives: " + newLives;

        if (lifeImages != null && lifeImages.Length > 0)
        {
            for (int i = 0; i < lifeImages.Length; i++)
            {
                var img = lifeImages[i];
                if (img == null) continue;

                if (i < newLives)
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
        
        if (!_isGameOver && !_isVictory && newLives < _lives)
        {
            if (damageFlash != null)
                damageFlash.Flash();
        }

        _lives = newLives;
    }

    public int InstanceMiss()
    {
        if (_isGameOver || _isVictory) return _lives;

        int newLives = _lives - 1;   // compute first
        UpdateLives(newLives);       // UpdateLives detects decrease correctly

        if (_lives <= 0)
            GameOver();

        return _lives;
    }

    public void GameOver()
    {
        if (_isGameOver) return;

        _isGameOver = true;

        PlaySfx(gameOverClip);

        Time.timeScale = 0f;

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + _score;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        SaveScore();
    }

    public void Victory()
    {
        if (_isVictory) return;

        _isVictory = true;

        PlaySfx(victoryClip);

        Time.timeScale = 0f;

        if (victoryScoreText != null)
            victoryScoreText.text = "Score: " + _score;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        SaveScore();
    }

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

    public void ContinueToNext()
    {
        Time.timeScale = 1f;

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("MainMenu");
    }

    // ---------- PRIVATE HELPERS ----------

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = _score + "/" + requiredScore;

        if (scoreFill != null)
        {
            float denom = Mathf.Max(1, requiredScore);
            scoreFill.fillAmount = Mathf.Clamp01((float)_score / denom);
        }
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt("LastLevelScore", _score);

        int total = PlayerPrefs.GetInt("TotalScore", 0);
        total += _score;
        PlayerPrefs.SetInt("TotalScore", total);

        PlayerPrefs.Save();
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;

        if (sfxSource == null)
        {
            // If you forgot the AudioSource, fail gracefully
            Debug.LogWarning("GameManager: No AudioSource assigned for SFX.");
            return;
        }

        float basePitch = 1f;
        sfxSource.pitch = basePitch + Random.Range(-pitchRandomness, pitchRandomness);
        sfxSource.PlayOneShot(clip, 1f);
    }
}
