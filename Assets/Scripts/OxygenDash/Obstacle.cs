using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 5f;
    public bool isWall = false; // special wall obstacle

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -10f)
            Destroy(gameObject);
    }

    // If player has shield → destroy bacteria when hit
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerOxygen player = other.GetComponent<PlayerOxygen>();
        if (player != null && player.IsShielded())
        {
            Destroy(gameObject);
        }
    }
}
