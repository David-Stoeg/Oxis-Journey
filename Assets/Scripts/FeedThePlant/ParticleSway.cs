using UnityEngine;

public class ParticleSway : MonoBehaviour
{
    public float swayAmplitude = 0.5f;
    public float swaySpeed = 2f;

    // Rotation settings (Grad)
    public float rotationAmplitude = 45f;
    public float rotationSpeed = 3f;
    public float rotationRandomness = 100f;


    // Rotationsachse im lokalen Raum (Standard: Z-Achse)
    public Vector3 rotationAxis = Vector3.forward;

    private float _startX;
    private float _offset;

    private Draggable _draggable;
    private bool _wasDragging = false;

    private Quaternion _startRotation;
    private float _rotationAmplitude;

    void Start()
    {
        _startX = transform.position.x;
        _offset = Random.Range(0f, 100f);

        _draggable = GetComponent<Draggable>();

        // Startrotation festhalten
        _startRotation = transform.rotation;
        _rotationAmplitude = rotationAmplitude + Random.Range(-rotationRandomness, rotationRandomness);

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
                _startRotation = transform.rotation;
                _wasDragging = false;
            }
        }

        // Apply sway normally
        Vector3 pos = transform.position;
        pos.x = _startX + Mathf.Sin((Time.time + _offset) * swaySpeed) * swayAmplitude;
        transform.position = pos;

        // Apply small local rotation around configured axis
        float rotAngle = Mathf.Sin((Time.time + _offset) * rotationSpeed) * rotationAmplitude;
        Quaternion rot = Quaternion.AngleAxis(rotAngle, rotationAxis.normalized);
        transform.rotation = _startRotation * rot;
    }
}
