using System.Collections;
using UnityEngine;

public class ShrinkAndDestroy : MonoBehaviour
{
    public float shrinkDuration = 0.15f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    bool _running = false;

    /// Call this instead of Destroy(gameObject)
    public void Play(System.Action onComplete = null)
    {
        if (_running) return;
        StartCoroutine(CoShrink(onComplete));
    }

    IEnumerator CoShrink(System.Action onComplete)
    {
        _running = true;

        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, shrinkDuration);
            float k = curve.Evaluate(t);
            transform.localScale = start * k;
            yield return null;
        }

        transform.localScale = Vector3.zero;

        onComplete?.Invoke();
        Destroy(gameObject);
    }
}
