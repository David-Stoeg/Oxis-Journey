using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Delivered O2! Minigame complete!");
            // transition to success screen
        }
    }
}
