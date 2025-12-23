using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class IntroSequence : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panel1;   // stays visible
    public GameObject panel3;
    public GameObject panel4;

    [Header("Fade-in Text (inside Panel 1)")]
    public TMP_Text panel1ExtraText; // drag the TMP text here (starts invisible)

    [Header("Timing")]
    public float holdSeconds = 2f;
    public float fadeDuration = 1f;

    private bool waitingForClick = false;

    void Start()
    {
        // Panels active so we can fade their graphics
        if (panel1) panel1.SetActive(true);
        if (panel3) panel3.SetActive(true);
        if (panel4) panel4.SetActive(true);

        // Initial alphas
        SetPanelAlpha(panel1, 1f);
        SetPanelAlpha(panel3, 0f);
        SetPanelAlpha(panel4, 0f);

        // Text starts hidden
        SetTMPAlpha(panel1ExtraText, 0f);

        StartCoroutine(RunSequence());
    }

    void Update()
    {
        if (waitingForClick && Input.anyKeyDown)
        {
            waitingForClick = false;
            LoadNextScene();
        }
    }

    IEnumerator RunSequence()
    {
        // Panel 1 already visible
        yield return new WaitForSecondsRealtime(holdSeconds);

        // Step 2: fade in ONLY the TMP text (still on panel 1)
        yield return FadeTMP(panel1ExtraText, 1f, fadeDuration);
        yield return new WaitForSecondsRealtime(holdSeconds);

        // Step 3: fade in panel 3
        yield return FadePanel(panel3, 1f, fadeDuration);
        yield return new WaitForSecondsRealtime(holdSeconds);

        // Step 4: fade in panel 4
        yield return FadePanel(panel4, 1f, fadeDuration);

        waitingForClick = true;
    }

    IEnumerator FadePanel(GameObject panel, float targetAlpha, float duration)
    {
        if (panel == null) yield break;

        Graphic[] graphics = panel.GetComponentsInChildren<Graphic>(true);
        if (graphics.Length == 0) yield break;

        float startAlpha = graphics[0].color.a;

        float t = 0f;
        float dur = Mathf.Max(0.01f, duration);

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / dur);

            for (int i = 0; i < graphics.Length; i++)
            {
                if (graphics[i] == null) continue;
                Color c = graphics[i].color;
                c.a = a;
                graphics[i].color = c;
            }
            yield return null;
        }

        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] == null) continue;
            Color c = graphics[i].color;
            c.a = targetAlpha;
            graphics[i].color = c;
        }
    }

    void SetPanelAlpha(GameObject panel, float alpha)
    {
        if (panel == null) return;

        Graphic[] graphics = panel.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] == null) continue;
            Color c = graphics[i].color;
            c.a = alpha;
            graphics[i].color = c;
        }
    }

    IEnumerator FadeTMP(TMP_Text tmp, float targetAlpha, float duration)
    {
        if (tmp == null) yield break;

        float startAlpha = tmp.color.a;

        float t = 0f;
        float dur = Mathf.Max(0.01f, duration);

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / dur);
            SetTMPAlpha(tmp, a);
            yield return null;
        }

        SetTMPAlpha(tmp, targetAlpha);
    }

    void SetTMPAlpha(TMP_Text tmp, float alpha)
    {
        if (tmp == null) return;
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }

    private void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("No next scene found in Build Settings.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextIndex);
    }
}
