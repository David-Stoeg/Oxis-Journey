using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlantTossDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        FallingObject obj = col.GetComponent<FallingObject>();
        if (obj != null)
        {
            Plant plant = GetComponent<Plant>();
            obj.ProcessAtPlant(plant);
        }
    }
}
