using UnityEngine;

public class Pollution : MonoBehaviour
{
    public float speed;
    public Vector3 direction;       // set by spawner
    public SunraySpawner spawner;

    void Update()
    {
        // Move in a straight line
        transform.position += direction * speed * Time.deltaTime;

        // If it just goes off-screen (outside despawnRadius) → no penalty, just disappear
        if (transform.position.magnitude > spawner.despawnRadius)
        {
            Die();
        }
    }

    // Called when the player clicks it (which is wrong!)
    public void Clicked()
    {
        GameManager.Instance.InstanceMiss();  // player shouldn't click pollution
        Die();
    }

    void Die()
    {
        if (spawner != null)
            spawner.ReleaseObject();

        Destroy(gameObject);
    }
}