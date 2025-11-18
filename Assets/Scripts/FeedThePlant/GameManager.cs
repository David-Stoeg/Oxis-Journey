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
    public Image scoreFill; // UI Image mit Fill (Type = Filled)

    [Header("Lives UI")]
    public Image[] lifeImages;           // Reihenfolge: links -> rechts (oder wie im UI angeordnet)
    public Sprite lifeFullSprite;       // Sprite für ein vorhandenes Leben
    public Sprite lifeEmptySprite;      // Sprite für ein verlorenes Leben

    [Header("Panels")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public GameObject victoryPanel;
    public TMP_Text victoryScoreText;

    [Header("Game Settings")]
    public int maxLives = 3;           
    public int requiredScore = 20;      // WIN CONDITION

    private int _score = 0;
    private int _lives;
    private bool _isGameOver = false;
    private bool _isVictory = false;

    // ---------- UNITY LIFECYCLE ----------

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        // No timer anymore
        if (_isGameOver || _isVictory) return;
    }

    // ---------- GAME FLOW ----------

    public void AddScore(int amount)
    {
        if (_isGameOver || _isVictory) return;

        _score += amount;
        UpdateScoreText();

        if (_score >= requiredScore)
        {
            Victory();
        }
    }

    public void UpdateLives(int newLives)
    {
        // Clamp zu 0 .. Anzahl der lifeImages (falls vorhanden) oder maxLives
        int maxDisplay = (lifeImages != null && lifeImages.Length > 0) ? lifeImages.Length : maxLives;
        newLives = Mathf.Clamp(newLives, 0, maxDisplay);
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
                    // Leben vorhanden
                    if (lifeFullSprite != null)
                    {
                        img.sprite = lifeFullSprite;
                        img.enabled = true;
                    }
                    else
                    {
                        img.enabled = true; // falls Sprite nicht gesetzt: sichtbar lassen
                    }
                }
                else
                {
                    // Leben verloren
                    if (lifeEmptySprite != null)
                    {
                        img.sprite = lifeEmptySprite;
                        img.enabled = true;
                    }
                    else
                    {
                        // kein Empty-Sprite: Bild ausblenden
                        img.enabled = false;
                    }
                }
            }
        }
            // interne Variable aktualisieren
            _lives = newLives;
       }

    public int InstanceMiss()
    {
        if (_isGameOver || _isVictory) return _lives;

        _lives--;
        UpdateLives(_lives);

        if (_lives <= 0)
        {
            GameOver();
        }

        return _lives;
    }

    public void GameOver()
    {
        if (_isGameOver) return;

        _isGameOver = true;
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
            //scoreText.text = "O2: " + _score + "/" + requiredScore;
            scoreText.text = _score + "/" + requiredScore; //mit neuer ScoreBar

        if (scoreFill != null)
        {
            // Schütze vor Division durch 0
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
}
