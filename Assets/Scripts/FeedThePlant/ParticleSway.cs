using UnityEngine;

public class ParticleSway : MonoBehaviour
{
    public float swayAmplitude = 0.5f;
    public float swaySpeed = 2f;

    private float _startX;
    private float _offset;

    private Draggable _draggable;
    private bool _wasDragging = false;

    void Start()
    {
        _startX = transform.position.x;
        _offset = Random.Range(0f, 100f);

        _draggable = GetComponent<Draggable>();
    }

    void Update()
    {
        if (_draggable != null)
        {
            // While dragging → pause sway
            if (_draggable.IsDragging)
            {
                _wasDragging = true;
                return;
            }

            // Dragging ended this frame → update sway origin ONCE
            if (_wasDragging)
            {
                _startX = transform.position.x;
                _wasDragging = false;
            }
        }

        // Apply sway normally
        Vector3 pos = transform.position;
        pos.x = _startX + Mathf.Sin((Time.time + _offset) * swaySpeed) * swayAmplitude;
        transform.position = pos;
    }
}
