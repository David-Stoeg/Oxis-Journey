using UnityEngine;

public class Sunray : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public SunraySpawner spawner;

    private bool resolved = false; // already scored/errored?

    void Update()
    {
        // Straight-line movement
        transform.position += direction * speed * Time.deltaTime;

        // Miss only when leaving screen area
        if (!resolved && transform.position.magnitude > spawner.despawnRadius)
        {
            // Only penalize if this object hasn't already caused an error
            if (!spawner.hasPenalizedThisObject)
            {
                GameManager.Instance.InstanceMiss();
                spawner.hasPenalizedThisObject = true;
            }

            resolved = true;
            Die();
        }
    }

    public void Absorb()
    {
        if (resolved) return;

        resolved = true;
        GameManager.Instance.AddScore(1);
        Die();
    }

    void Die()
    {
        if (spawner != null)
            spawner.ReleaseObject();

        Destroy(gameObject);
    }
}
