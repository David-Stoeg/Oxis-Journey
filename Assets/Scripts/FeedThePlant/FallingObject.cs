using UnityEngine;

public enum FallingType { Good, Bad }

public class FallingObject : MonoBehaviour
{
    public FallingType type = FallingType.Good;
    public int scoreValue = 1;
    public int damageValue = 1;

    private bool _processed = false;

    public void ProcessAtPlant(Plant plant)
    {
        if (_processed) return;
        _processed = true;

        if (type == FallingType.Good)
            GameManager.Instance.AddScore(scoreValue);
        else
            plant.TakeDamage(damageValue);

        Destroy(gameObject);
    }

    void Update()
    {
        if (transform.position.y < -6f)
            Destroy(gameObject);
    }
}
