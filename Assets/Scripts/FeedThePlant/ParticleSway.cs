using UnityEngine;

public class ParticleSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmplitude = 0.5f;
    public float swaySpeed = 2f;
    public float rotationAmplitude = 45f;
    public float rotationSpeed = 3f;
    public float rotationRandomness = 100f;
    public Vector3 rotationAxis = Vector3.forward;

    [Header("Falling Physics")]
    public float idleGravity = 0.3f;
    public float tossGravity = 1.2f;
    public float maxFallSpeed = 3f;

    private Rigidbody2D _rb;
    private Draggable _drag;

    private float _offset;
    private bool _wasDragging = false;
    private bool _isTossed = false;

    private Quaternion _baseRot;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _drag = GetComponent<Draggable>();

        _offset = Random.Range(0f, 100f);
        _baseRot = transform.rotation;

        _rb.gravityScale = idleGravity;
    }

    void Update()
    {
        // ---------------- DRAGGING ----------------
        if (_drag.IsDragging)
        {
            _wasDragging = true;
            _isTossed = false;

            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0f;

            return;
        }

        // ------------- AFTER DRAGGING --------------
        if (_wasDragging)
        {
            _wasDragging = false;

            // Released with speed → tossed
            if (_rb.linearVelocity.magnitude > 0.1f && _drag.WasJustReleased)
            {
                _isTossed = true;
                _rb.gravityScale = tossGravity;
                return;
            }

            // Released gently → idle floating
            _isTossed = false;
            _rb.gravityScale = idleGravity;
        }

        // ---------------- TOSSED ----------------
        if (_isTossed)
        {
            // Hard falling, no sway
            LimitFallSpeed();
            return;
        }

        // ---------------- IDLE FLOATING (FIXED!) ----------------
        ApplyIdleSway();
        LimitFallSpeed();
    }

    void ApplyIdleSway()
    {
        // LEFT-RIGHT sway added on top of physics
        float sway = Mathf.Sin((Time.time + _offset) * swaySpeed) * swayAmplitude;

        Vector3 pos = transform.position;
        pos.x += sway * Time.deltaTime;   // additive sway
        transform.position = pos;

        // ROTATION sway
        float rotAngle = Mathf.Sin((Time.time + _offset) * rotationSpeed) * rotationAmplitude;
        transform.rotation = _baseRot * Quaternion.AngleAxis(rotAngle, rotationAxis);
    }

    void LimitFallSpeed()
    {
        Vector2 v = _rb.linearVelocity;

        if (v.y < -maxFallSpeed)
            v.y = -maxFallSpeed;

        _rb.linearVelocity = v;
    }
}
