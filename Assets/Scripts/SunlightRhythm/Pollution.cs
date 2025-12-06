using UnityEngine;

public class Pollution : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public SunraySpawner spawner;

    private bool resolved = false;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Leaving screen → just despawn, no penalty
        if (transform.position.magnitude > spawner.despawnRadius)
        {
            Die();
        }
    }

    // Called when player clicks pollution over the leaf
    public void Clicked()
    {
        if (resolved) return;

        // Only penalize once per object
        if (!spawner.hasPenalizedThisObject)
        {
            GameManager.Instance.InstanceMiss();
            spawner.hasPenalizedThisObject = true;
        }

        resolved = true;
        Die();
    }

    void Die()
    {
        if (spawner != null)
            spawner.ReleaseObject();

        Destroy(gameObject);
    }
}
