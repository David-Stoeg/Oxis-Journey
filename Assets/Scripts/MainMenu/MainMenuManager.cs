using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Called by the Start button
    public void StartGame()
    {
        // Reset the total or last score if needed
        PlayerPrefs.SetInt("LastLevelScore", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("FTP-Tutorial"); // make sure this matches your scene name exactly
    }

    // Called by the Exit button
    public void ExitGame()
    {
#if UNITY_EDITOR
        // If in Unity Editor, stop Play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // In a built game, quit application
        Application.Quit();
#endif
    }
}
