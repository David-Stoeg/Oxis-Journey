using UnityEngine;

public class HemoglobinBoost : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().ActivateInvincibility();
            Destroy(gameObject);
        }
    }
}
