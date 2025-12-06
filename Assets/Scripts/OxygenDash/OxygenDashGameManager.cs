using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OxygenDashGameManager : MonoBehaviour
{
    public static OxygenDashGameManager Instance;

    [Header("Lives")]
    public int maxLives = 3;
    private int lives;

    [Header("UI")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public TMP_Text livesText;

    private bool gameEnded = false;

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
        lives = maxLives;
        UpdateLivesUI();

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
        UpdateLivesUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    // -------------------------------
    // PUBLIC: called when reaching the end cell
    // -------------------------------
    public void Victory()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (victoryPanel)
            victoryPanel.SetActive(true);

        Time.timeScale = 0f;   // FREEZE GAME TOO
    }

    // -------------------------------
    void GameOver()
    {
        gameEnded = true;

        if (gameOverPanel)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;   // FREEZE GAME
    }
    
    private void UpdateLivesUI()
    {
        if (livesText)
            livesText.text = "Lives: " + lives;
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;   // unfreeze the game
        SceneManager.LoadScene("MainMenu");
    }
}
