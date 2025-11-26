using UnityEngine;

public class Sunray : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public SunraySpawner spawner;

    private bool absorbed = false;

    void Update()
    {
        // Straight-line movement
        transform.position += direction * speed * Time.deltaTime;

        // Miss only when leaving screen area
        if (!absorbed && transform.position.magnitude > spawner.despawnRadius)
        {
            GameManager.Instance.InstanceMiss();
            Die();
        }
    }

    public void Absorb()
    {
        if (absorbed) return;

        absorbed = true;
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
