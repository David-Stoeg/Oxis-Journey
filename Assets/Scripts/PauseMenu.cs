using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject pausePanel;

    private bool _isPaused = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false); // hide on start
    }

    // Called by Pause Button
    public void TogglePause()
    {
        if (_isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (_isPaused) return;

        _isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void Resume()
    {
        if (!_isPaused) return;

        _isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    // Called by Exit button
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
