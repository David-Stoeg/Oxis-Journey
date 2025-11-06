using UnityEngine;

[RequireComponent(typeof(Collider2D))]
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
        // If player is holding it ABOVE plant then releases,
        // OnMouseUp will re-enable gravity BUT object is still overlapping.
        // We'll allow "delivery" when the object is *not currently being dragged*.

        FallingObject falling = other.GetComponent<FallingObject>();
        Draggable drag = other.GetComponent<Draggable>();

        if (falling != null && drag != null)
        {
            // delivered only if not being dragged right now
            // (prevents scoring spam while you're still holding it)
            if (!IsDragging(drag))
            {
                falling.ProcessAtPlant(this);
            }
        }
    }

    private bool IsDragging(Draggable drag)
    {
        // We can't access _isDragging directly (private),
        // so let's make a public helper in Draggable instead.
        return drag.IsDragging;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        GameManager.Instance.UpdateLives(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // simple lose condition for now
        Debug.Log("Plant died. Game Over.");
        GameManager.Instance.GameOver();
    }
}
