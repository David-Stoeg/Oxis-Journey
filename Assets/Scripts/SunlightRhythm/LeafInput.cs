using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class LeafInput : MonoBehaviour
{
    private Camera cam;

    private Sunray currentSunray;
    private Pollution currentPollution;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TryClick();

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            TryClick();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Sunray ray = col.GetComponent<Sunray>();
        if (ray != null)
        {
            currentSunray = ray;
            return;
        }

        Pollution pol = col.GetComponent<Pollution>();
        if (pol != null)
        {
            currentPollution = pol;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Sunray ray = col.GetComponent<Sunray>();
        if (ray != null && ray == currentSunray)
        {
            currentSunray = null;
            return;
        }

        Pollution pol = col.GetComponent<Pollution>();
        if (pol != null && pol == currentPollution)
        {
            currentPollution = null;
        }
    }

    void TryClick()
    {
        // 🛑 If the click is on UI, IGNORE it
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // ✔ Real game click
        if (currentSunray != null)
        {
            currentSunray.Absorb();
        }
        else if (currentPollution != null)
        {
            currentPollution.Clicked();
        }
        else
        {
            GameManager.Instance.InstanceMiss();
        }
    }
}