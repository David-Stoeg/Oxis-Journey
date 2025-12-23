using UnityEngine;

public class Pollution : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public SunraySpawner spawner;

    private bool resolved = false;

    private ShrinkAndDestroy shrink;

    void Awake()
    {
        shrink = GetComponent<ShrinkAndDestroy>(); // add this component on prefab
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Leaving screen → just despawn, no penalty
        if (spawner != null && transform.position.magnitude > spawner.despawnRadius)
        {
            resolved = true;
            DieAnimated(); // ✅ shrink out (optional, looks nice)
        }
    }

    // Called when player clicks pollution over the leaf
    public void Clicked()
    {
        if (resolved) return;

        // Only penalize once per object
        if (spawner != null && !spawner.hasPenalizedThisObject)
        {
            GameManager.Instance.InstanceMiss();
            spawner.hasPenalizedThisObject = true;
        }

        resolved = true;
        DieAnimated(); // ✅ shrink out on error
    }

    void DieAnimated()
    {
        if (shrink == null)
        {
            DieImmediate();
            return;
        }

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
