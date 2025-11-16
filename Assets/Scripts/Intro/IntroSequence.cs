using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    public CanvasGroup intro;
    public CanvasGroup titleText;
    public CanvasGroup finalScreen;

    public float fadeDuration = 1f;

    private bool waitingForClick = false;

    void Start()
    {
        StartCoroutine(RunSequence());
    }

    void Update()
    {
        // Wait for click → load next scene
        if (waitingForClick && Input.anyKeyDown)
        {
            waitingForClick = false;
            LoadNextScene();
        }
    }

    IEnumerator RunSequence()
    {
        // Intro already visible by default
        yield return new WaitForSeconds(2f);

        // Fade in TitleText
        yield return StartCoroutine(Fade(titleText, 1f, fadeDuration));
        yield return new WaitForSeconds(2f);

        // Fade in FinalScreen
        yield return StartCoroutine(Fade(finalScreen, 1f, fadeDuration));

        // Now wait for click
        waitingForClick = true;
    }

    IEnumerator Fade(CanvasGroup group, float target, float duration)
    {
        float start = group.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }

        group.alpha = target;
    }

    private void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("No next scene found in Build Settings.");
            return;
        }

        SceneManager.LoadScene(nextIndex);
    }
}
