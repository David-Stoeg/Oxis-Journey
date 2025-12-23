using System.Collections;
using UnityEngine;

public class ScalePulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float scaleMultiplier = 1.2f; // 1.10–1.30 usually feels good
    public float pulseTime = 0.18f;      // total time up+down

    private Vector3 baseScale;
    private Coroutine routine;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    public void Pulse()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PulseRoutine());
    }

    IEnumerator PulseRoutine()
    {
        baseScale = transform.localScale;

        float half = Mathf.Max(0.01f, pulseTime * 0.5f);
        Vector3 peak = baseScale * scaleMultiplier;

        // up
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            transform.localScale = Vector3.Lerp(baseScale, peak, p);
            yield return null;
        }

        // down
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            transform.localScale = Vector3.Lerp(peak, baseScale, p);
            yield return null;
        }

        transform.localScale = baseScale;
        routine = null;
    }
}
