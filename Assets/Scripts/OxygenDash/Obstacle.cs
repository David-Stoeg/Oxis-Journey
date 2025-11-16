using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -10f)
            Destroy(gameObject);
    }
}
