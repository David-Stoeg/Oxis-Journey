using UnityEngine;

public class Plant : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Minigame Feedback")]
    public PlantMinigameFeedback feedback;

    private void Awake()
    {
        currentHealth = maxHealth;

        // ✅ auto-grab if you forgot to drag it
        if (feedback == null)
            feedback = GetComponent<PlantMinigameFeedback>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Hit"))
            return;

        FallingObject falling = other.GetComponentInParent<FallingObject>();
        Draggable drag = other.GetComponentInParent<Draggable>();

        if (falling == null) return;

        if (drag != null && drag.IsDragging)
        {
            if (falling.type == FallingType.Good)
            {
                falling.ProcessAtPlant(this);
                return;
            }
        }

        if (drag == null || !drag.IsDragging)
        {
            falling.ProcessAtPlant(this);
        }
    }

    public void OnFedGood(int scoreAmount = 1)
    {
        if (feedback != null)
            feedback.Grow(scoreAmount);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        GameManager.Instance.UpdateLives(currentHealth);

        if (feedback != null)
            feedback.Error(); // ✅ flash red on error

        if (currentHealth <= 0)
            GameManager.Instance.GameOver();
    }
}
