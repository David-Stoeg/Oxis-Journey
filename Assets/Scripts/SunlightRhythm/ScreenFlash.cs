using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFlash : MonoBehaviour
{
    public float maxAlpha = 0.35f;
    public float fadeInTime = 0.05f;
    public float fadeOutTime = 0.20f;

    private Image img;
    private Coroutine routine;

    void Awake()
    {
        img = GetComponent<Image>();
        img.raycastTarget = false;

        // start invisible
        var c = img.color;
        c.a = 0f;
        img.color = c;
    }

    public void Flash()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        yield return FadeTo(maxAlpha, fadeInTime);
        yield return FadeTo(0f, fadeOutTime);
        routine = null;
    }

    IEnumerator FadeTo(float target, float time)
    {
        float start = img.color.a;
        float t = 0f;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float p = (time <= 0f) ? 1f : Mathf.Clamp01(t / time);

            var c = img.color;
            c.a = Mathf.Lerp(start, target, p);
            img.color = c;

            yield return null;
        }

        var c2 = img.color;
        c2.a = target;
        img.color = c2;
    }
}
