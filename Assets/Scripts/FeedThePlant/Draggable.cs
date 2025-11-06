using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Camera _cam;
    private Rigidbody2D _rb;
    private Vector3 _offset;
    private bool _isDragging;

    public bool IsDragging => _isDragging;

    void Awake()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;

        if (_rb != null)
        {
            _rb.gravityScale = 0f;
            _rb.linearVelocity = Vector2.zero;
        }

        Vector3 worldPos = _cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0f;
        _offset = transform.position - worldPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        Vector3 worldPos = _cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0f;
        transform.position = worldPos + _offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;

        if (_rb != null)
        {
            _rb.gravityScale = 0.5f; // same default as before
        }
    }
}
