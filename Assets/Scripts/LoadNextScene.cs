using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public void LoadNext()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Safety check so Unity doesn't crash if you're on the last scene
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextIndex);
        }
        else
            Debug.LogError("No next scene in Build Settings!");
    }
}
