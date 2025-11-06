using UnityEngine;

public class Sunray : MonoBehaviour
{
    public float speed = 5f;
    public bool fromLeft = true;
    private bool _scored = false;

    private void Update()
    {
        float direction = fromLeft ? 1f : -1f;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // destroy if off-screen
        if (Mathf.Abs(transform.position.x) > 10f)
        {
            if (!_scored)
                GameManager.Instance.InstanceMiss(); // ✅ fixed
            Destroy(gameObject);
        }
    }

    public void Absorb()
    {
        if (_scored) return;
        _scored = true;
        GameManager.Instance.AddScore(1);
        Destroy(gameObject);
    }
}
