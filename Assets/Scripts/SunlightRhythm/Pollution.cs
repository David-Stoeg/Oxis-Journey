using UnityEngine;

public class Pollution : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public SunraySpawner spawner;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Leaving screen: no penalty, just despawn
        if (transform.position.magnitude > spawner.despawnRadius)
        {
            Die();
        }
    }

    public void Clicked()
    {
        // Player should NOT click pollution
        GameManager.Instance.InstanceMiss();
        Die();
    }

    void Die()
    {
        if (spawner != null)
            spawner.ReleaseObject();

        Destroy(gameObject);
    }
}
