using UnityEngine;

public class Sunray : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public SunraySpawner spawner;

    private bool resolved = false;

    // pulse target
    private ScalePulse pulseTarget;

    // NEW
    private ShrinkAndDestroy shrink;

    public void SetPulseTarget(ScalePulse target) => pulseTarget = target;

    void Awake()
    {
        shrink = GetComponent<ShrinkAndDestroy>(); // should be on prefab
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (!resolved && spawner != null && transform.position.magnitude > spawner.despawnRadius)
        {
            if (!spawner.hasPenalizedThisObject)
            {
                GameManager.Instance.InstanceMiss();
                spawner.hasPenalizedThisObject = true;
            }

            resolved = true;
            DieAnimated();
        }
    }

    public void Absorb()
    {
        if (resolved) return;
        resolved = true;

        GameManager.Instance.AddScore(1);

        if (pulseTarget != null)
            pulseTarget.Pulse();

        DieAnimated(); // ✅ shrink out on score
    }

    // Call this for any error case too if you have one
    public void Error()
    {
        if (resolved) return;
        resolved = true;

        GameManager.Instance.InstanceMiss(); // if that's your “error” for this minigame
        DieAnimated(); // ✅ shrink out on error
    }

    void DieAnimated()
    {
        // If no shrink script, fallback
        if (shrink == null)
        {
            DieImmediate();
            return;
        }

        // shrink, then release, then destroy is already handled by ShrinkAndDestroy
        shrink.Play(onComplete: () =>
        {
            if (spawner != null)
                spawner.ReleaseObject();
        });
    }

    void DieImmediate()
    {
        if (spawner != null)
            spawner.ReleaseObject();

        Destroy(gameObject);
    }
}
