using UnityEngine;
using UnityEngine.InputSystem;

public class LeafInput : MonoBehaviour
{
    private Collider2D _zoneCollider;
    private Camera _cam;

    void Awake()
    {
        _zoneCollider = GetComponent<Collider2D>();
        _cam = Camera.main;
    }

    void OnEnable()
    {
        // subscribe to global click/tap events
        InputSystem.onActionChange += OnActionChange;
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    void Update()
    {
        // works for mouse and touch
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAbsorb();
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            TryAbsorb();
        }
    }

    void TryAbsorb()
    {
        float radius = ((CircleCollider2D)_zoneCollider).radius * transform.localScale.x;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        bool absorbed = false;
        foreach (var hit in hits)
        {
            Sunray ray = hit.GetComponent<Sunray>();
            if (ray != null)
            {
                ray.Absorb();
                absorbed = true;
            }
        }

        if (!absorbed)
        {
            GameManager.Instance.InstanceMiss();
        }
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        // placeholder if you later add proper Input Actions
    }
}
