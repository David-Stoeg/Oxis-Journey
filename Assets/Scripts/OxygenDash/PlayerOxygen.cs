using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float verticalLimit = 4f;

    private Rigidbody2D rb;
    
    // Shield (you may remove this if unused)
    private bool isShielded = false;
    private float invincibilityTimer = 0f;
    
    [Header("Drag Settings")]
    public float dragDeadZone = 0.1f;     // mouse must move this far to trigger motion
    public float dragSmooth = 10f;        // smoothing multiplier

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float vertical = 0f;

        // ---------------------------------
        // KEYBOARD SUPPORT (optional)
        // ---------------------------------
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed ||
                UnityEngine.InputSystem.Keyboard.current.upArrowKey.isPressed)
                vertical = 1;

            else if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed ||
                     UnityEngine.InputSystem.Keyboard.current.downArrowKey.isPressed)
                vertical = -1;
        }

        // ---------------------------------
        // FAST DRAG FOLLOW (NO JITTER)
        // ---------------------------------
        if (UnityEngine.InputSystem.Mouse.current != null &&
            UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(
                UnityEngine.InputSystem.Mouse.current.position.ReadValue()
            );
            mouse.z = 0;

            float delta = mouse.y - transform.position.y;

            // Apply deadzone to avoid jitter
            if (Mathf.Abs(delta) > dragDeadZone)
            {
                // Move toward mouse at full moveSpeed, no smoothing delay
                vertical = Mathf.Sign(delta);
            }
            else
            {
                vertical = 0;
            }
        }

        // ---------------------------------
        // APPLY MOVEMENT
        // ---------------------------------
        Vector2 newPos = rb.position + Vector2.up * vertical * moveSpeed * Time.deltaTime;
        newPos.y = Mathf.Clamp(newPos.y, -verticalLimit, verticalLimit);
        rb.MovePosition(newPos);

        if (invincibilityTimer > 0)
            invincibilityTimer -= Time.deltaTime;
    }

    // ---------------------------------------------------------
    //     MERGED TRIGGER HANDLING (ONLY ONE FUNCTION NOW)
    // ---------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ====== BACTERIA (Damage) ======
        if (other.CompareTag("Obstacle"))
        {
            if (isShielded)
            {
                // Shield takes the hit
                isShielded = false;
                invincibilityTimer = 1f;
            }
            else if (invincibilityTimer <= 0f)
            {
                // Lose a life (NEW GameManager)
                OxygenDashGameManager.Instance.LoseLife();
                invincibilityTimer = 1f;
            }

            Destroy(other.gameObject);
            return;
        }

        // ====== BOOST (optional) ======
        if (other.CompareTag("Boost"))
        {
            ActivateShield();
            Destroy(other.gameObject);
            return;
        }

        // ====== END CELL (Victory) ======
        if (other.CompareTag("Finish"))
        {
            OxygenDashGameManager.Instance.Victory();
            return;
        }
    }

    // Optional shield logic
    void ActivateShield()
    {
        isShielded = true;
        invincibilityTimer = 0f;
        // TODO: visual effect
    }

    public bool IsShielded()
    {
        return isShielded;
    }
}
