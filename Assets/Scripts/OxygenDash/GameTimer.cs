using UnityEngine;
using TMPro;   // << IMPORTANT

public class GameTimer : MonoBehaviour
{
    public float gameDuration = 30f;
    public TMP_Text timerText;   // << IMPORTANT
    public bool finished = false;

    private float timer;

    void Start()
    {
        timer = gameDuration;
    }

    void Update()
    {
        if (finished) return;

        timer -= Time.deltaTime;

        timerText.text = timer.ToString("F1");

        if (timer <= 0)
        {
            finished = true;
            Debug.Log("Time’s up! Show final blood cell.");
        }
    }
}
