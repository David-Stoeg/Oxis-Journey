using System.Collections;
using UnityEngine;

public class PlantMinigameFeedback : MonoBehaviour
{
    [Header("Error Feedback")]
    public ScreenFlash errorFlash;

    [Header("Growth")]
    public Transform plantRoot;              // leave empty -> auto uses this.transform
    public float growthPerScore = 0.05f;     // 0.05 = +5% per correct
    public float maxGrowthMultiplier = 2f;   // max size relative to start

    [Header("Animation")]
    public float growDuration = 0.12f;

    private Vector3 _startScale;
    private Coroutine _routine;

    void Awake()
    {
        if (plantRoot == null)
            plantRoot = transform;           // ✅ scale the whole Plant object

        _startScale = plantRoot.localScale;
    }

    public void Error()
    {
        if (errorFlash != null)
            errorFlash.Flash();
    }

    public void Grow(int amount = 1)
    {
        if (plantRoot == null) return;

        float multStep = 1f + (growthPerScore * amount);
        Vector3 target = plantRoot.localScale * multStep;

        // clamp to startScale * maxGrowthMultiplier
        Vector3 maxScale = _startScale * Mathf.Max(1f, maxGrowthMultiplier);
        target = new Vector3(
            Mathf.Min(target.x, maxScale.x),
            Mathf.Min(target.y, maxScale.y),
            Mathf.Min(target.z, maxScale.z)
        );

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(ScaleTo(target));
    }

    IEnumerator ScaleTo(Vector3 target)
    {
        Vector3 from = plantRoot.localScale;
        float t = 0f;
        float dur = Mathf.Max(0.01f, growDuration);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            plantRoot.localScale = Vector3.Lerp(from, target, t);
            yield return null;
        }

        plantRoot.localScale = target;
        _routine = null;
    }
}
