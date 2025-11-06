using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject goodPrefab; // O2
    public GameObject badPrefab;  // Pollution/Bug

    public float spawnInterval = 1.0f;
    public float xRange = 7f;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        // 70% good, 30% bad (tweak later)
        bool spawnGood = Random.value < 0.7f;

        GameObject prefabToUse = spawnGood ? goodPrefab : badPrefab;

        Vector3 pos = transform.position;
        pos.x = Random.Range(-xRange, xRange);

        Instantiate(prefabToUse, pos, Quaternion.identity);
    }
}
