using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Collider2D grabCollider;

    private Camera _cam;
    private Rigidbody2D _rb;
    private Vector3 _offset;
    private bool _isDragging;

    private Vector3 _lastMousePos;
    private Vector3 _velocity;

    public bool IsDragging => _isDragging;
    public bool WasJustReleased { get; private set; }

    [Header("Toss Settings")]
    public float tossMultiplier = 8f;

    void Awake()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsPointerOverGrabCollider(eventData)) return;

        _isDragging = true;
        WasJustReleased = false;

        // STOP ALL MOTION immediately
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0f;

        Vector3 worldPos = _cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0f;

        _offset = transform.position - worldPos;
        _lastMousePos = worldPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        Vector3 worldPos = _cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0f;

        _velocity = (worldPos - _lastMousePos) / Time.deltaTime;
        _lastMousePos = worldPos;

        transform.position = worldPos + _offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging) return;

        _isDragging = false;
        WasJustReleased = true;

        // Apply flick velocity
        _rb.linearVelocity = _velocity * tossMultiplier * Time.fixedDeltaTime;
        _rb.gravityScale = 1f; // resume gravity for falling if needed
    }

    private bool IsPointerOverGrabCollider(PointerEventData eventData)
    {
        Vector3 worldPos = _cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0f;

        return grabCollider != null && grabCollider.OverlapPoint(worldPos);
    }
}
