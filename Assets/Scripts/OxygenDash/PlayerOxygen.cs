using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float verticalLimit = 4f;

    private Rigidbody2D rb;
    private bool isShielded = false;
    private float invincibilityTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float vertical = 0f;

        // Keyboard (Input System)
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed ||
                UnityEngine.InputSystem.Keyboard.current.upArrowKey.isPressed)
                vertical = 1;

            else if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed ||
                     UnityEngine.InputSystem.Keyboard.current.downArrowKey.isPressed)
                vertical = -1;
        }

        // Mouse (left = up, right = down)
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            if (UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
                vertical = 1;
            if (UnityEngine.InputSystem.Mouse.current.rightButton.isPressed)
                vertical = -1;
        }

        Vector2 newPos = rb.position + Vector2.up * vertical * moveSpeed * Time.deltaTime;
        newPos.y = Mathf.Clamp(newPos.y, -verticalLimit, verticalLimit);

        rb.MovePosition(newPos);

        if (invincibilityTimer > 0)
            invincibilityTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (isShielded)
            {
                isShielded = false;
                invincibilityTimer = 1f;
            }
            else if (invincibilityTimer <= 0)
            {
                GameManager.Instance.InstanceMiss();
                invincibilityTimer = 1f;
            }

            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Boost"))
        {
            ActivateShield();
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Finish"))
        {
            GameManager.Instance.Victory();
        }
    }

    void ActivateShield()
    {
        isShielded = true;
        invincibilityTimer = 0f;
        // TODO: glow effect
    }
}
