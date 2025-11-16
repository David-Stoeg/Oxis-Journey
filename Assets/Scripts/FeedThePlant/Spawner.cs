using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject goodPrefab;     // O2
    public GameObject[] badPrefabs;   // Pollution/Bug (up to 3)

    [Header("Settings")]
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
        // 70% good, 30% bad
        bool spawnGood = Random.value < 0.6f;

        GameObject prefabToUse;

        if (spawnGood || badPrefabs == null || badPrefabs.Length == 0)
        {
            // fallback: if no bad prefabs assigned, always spawn good
            prefabToUse = goodPrefab;
        }
        else
        {
            // choose a random bad prefab from available ones
            prefabToUse = badPrefabs[Random.Range(0, badPrefabs.Length)];
        }

        Vector3 pos = transform.position;
        pos.x = Random.Range(-xRange, xRange);

        GameObject spawned = Instantiate(prefabToUse, pos, Quaternion.identity);

        // Add sway if object doesn't already have it
        if (spawned.GetComponent<ParticleSway>() == null)
            spawned.AddComponent<ParticleSway>();
    }
}
