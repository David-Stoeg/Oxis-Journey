using UnityEngine;

public enum FallingType
{
    Good,
    Bad
}

public class FallingObject : MonoBehaviour
{
    public FallingType type = FallingType.Good;
    public int scoreValue = 1;   // how many points if delivered (for Good)
    public int damageValue = 1;  // how much damage to plant if delivered (for Bad)

    private bool _hasBeenProcessed = false;

    public void ProcessAtPlant(Plant plant)
    {
        if (_hasBeenProcessed) return; // avoid double scoring
        _hasBeenProcessed = true;

        if (type == FallingType.Good)
        {
            GameManager.Instance.AddScore(scoreValue);
        }
        else // Bad
        {
            plant.TakeDamage(damageValue);
        }

        Destroy(gameObject);
    }

    // Optional: if it falls off-screen, destroy it so we don't leak objects
    void Update()
    {
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}
