using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalSceneButtons : MonoBehaviour
{
    // Go back to main menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Exit game OR stop Unity Play Mode
    public void ExitGame()
    {
        Debug.Log("Exit pressed");

        // If running inside Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
