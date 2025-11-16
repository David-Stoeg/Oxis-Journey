using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private bool invincible = false;
    private float invincibleTimer = 0f;
    public float invincibleDuration = 3f;

    public int maxLives = 3;
    private int currentLives;

    void Start()
    {
        currentLives = maxLives;
    }

    void Update()
    {
        HandleMovement();
        HandleInvincibility();
    }

    void HandleMovement()
    {
        float vertical = 0f;

        // Keyboard input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vertical = 1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vertical = -1f;

        // Mouse input (right = up, left = down)
        if (Input.GetMouseButton(1)) // right mouse
            vertical = 1f;
        if (Input.GetMouseButton(0)) // left mouse
            vertical = -1f;

        transform.Translate(Vector3.up * vertical * moveSpeed * Time.deltaTime);
    }

    void HandleInvincibility()
    {
        if (invincible)
        {
            invincibleTimer -= Time.deltaTime;

            if (invincibleTimer <= 0)
                invincible = false;
        }
    }

    public void ActivateInvincibility()
    {
        invincible = true;
        invincibleTimer = invincibleDuration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            if (!invincible)
            {
                currentLives--;
                Debug.Log("Hit! Lives left: " + currentLives);

                if (currentLives <= 0)
                    Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Reload level or show game over screen
    }
}
