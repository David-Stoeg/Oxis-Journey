using UnityEngine;

public class SunraySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject sunrayPrefab;
    public GameObject pollutionPrefab;

    [Header("Spawn Settings")]
    public float spawnRadius = 10f;
    public float despawnRadius = 12f;

    // Only fast speeds now
    public float[] speedStages = { 10f, 14f };

    [Range(0f, 1f)]
    public float pollutionChance = 0.3f;   // 30% pollution, 70% sunrays

    private GameObject activeObject;

    void Start()
    {
        SpawnObject();
    }

    void Update()
    {
        if (activeObject == null)
            SpawnObject();
    }

    public void SpawnObject()
    {
        // Random angle on spawn circle
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;

        Vector3 spawnPos = new Vector3(
            Mathf.Cos(rad),
            Mathf.Sin(rad),
            0f
        ) * spawnRadius;

        bool spawnPollution = Random.value < pollutionChance;

        if (spawnPollution)
        {
            // Spawn pollution
            activeObject = Instantiate(pollutionPrefab, spawnPos, Quaternion.identity);

            Pollution p = activeObject.GetComponent<Pollution>();
            Vector3 direction = (-spawnPos).normalized;   // goes through leaf center

            p.direction = direction;
            p.speed = speedStages[Random.Range(0, speedStages.Length)];
            p.spawner = this;
        }
        else
        {
            // Spawn sunray
            activeObject = Instantiate(sunrayPrefab, spawnPos, Quaternion.identity);

            Sunray ray = activeObject.GetComponent<Sunray>();
            Vector3 direction = (-spawnPos).normalized;  // goes through leaf center

            ray.direction = direction;
            ray.speed = speedStages[Random.Range(0, speedStages.Length)];
            ray.spawner = this;
        }
    }

    public void ReleaseObject()
    {
        activeObject = null;
    }
}