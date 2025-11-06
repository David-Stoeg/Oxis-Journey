using UnityEngine;

public class SunraySpawner : MonoBehaviour
{
    public GameObject sunrayPrefab;
    public float minInterval = 0.8f;
    public float maxInterval = 1.8f;
    public float spawnY = 0f;

    private float _timer = 0f;
    private float _nextSpawn = 1f;

    void Start()
    {
        ScheduleNext();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _nextSpawn)
        {
            SpawnSunray();
            ScheduleNext();
        }
    }

    void ScheduleNext()
    {
        _timer = 0f;
        _nextSpawn = Random.Range(minInterval, maxInterval);
    }

    void SpawnSunray()
    {
        bool fromLeft = Random.value > 0.5f;
        float startX = fromLeft ? -9f : 9f;
        Vector3 pos = new Vector3(startX, spawnY, 0f);

        GameObject ray = Instantiate(sunrayPrefab, pos, Quaternion.identity);
        Sunray sunray = ray.GetComponent<Sunray>();
        sunray.fromLeft = fromLeft;
        sunray.speed = Random.Range(3f, 6f);
    }
}
