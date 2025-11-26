using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class LeafInput : MonoBehaviour
{
    private Sunray currentSunray;
    private Pollution currentPollution;

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
            currentPollution = pol;
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
            currentPollution = null;
    }

    void TryClick()
    {
        // Ignore UI clicks
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // Good click → absorb sunlight
        if (currentSunray != null)
        {
            currentSunray.Absorb();
            return;
        }

        // Bad click → pollution clicked
        if (currentPollution != null)
        {
            currentPollution.Clicked();
            return;
        }

        // Clicked nothing relevant
        GameManager.Instance.InstanceMiss();
    }
}
