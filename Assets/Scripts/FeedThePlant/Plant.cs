using UnityEngine;

public class Plant : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Hit"))
            return;

        FallingObject falling = other.GetComponentInParent<FallingObject>();
        Draggable drag = other.GetComponentInParent<Draggable>();

        if (falling == null) return;

        // DRAGGING → check if auto-collect CO2
        if (drag != null && drag.IsDragging)
        {
            if (falling.type == FallingType.Good)
            {
                falling.ProcessAtPlant(this); // auto score
                return;
            }
        }

        // NOT DRAGGING → normal process
        if (drag == null || !drag.IsDragging)
        {
            falling.ProcessAtPlant(this);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        GameManager.Instance.UpdateLives(currentHealth);

        if (currentHealth <= 0)
            GameManager.Instance.GameOver();
    }
}
