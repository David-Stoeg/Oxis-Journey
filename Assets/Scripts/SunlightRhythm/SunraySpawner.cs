using UnityEngine;

public class SunraySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject sunrayPrefab;
    public GameObject pollutionPrefab;

    [Header("Spawn Settings")]
    public float spawnRadius = 10f;
    public float despawnRadius = 12f;

    // Only fast speeds for normal rays
    public float[] speedStages = { 10f, 14f };

    [Header("Pollution Settings")]
    [Range(0f, 1f)]
    public float pollutionChance = 0.3f;

    [Header("First Ray Settings")]
    public bool firstRaySpawned = false;
    public float firstRayDelay = 2f;
    public float firstRaySpeed = 5f;

    [HideInInspector] public GameObject activeObject;

    // 👇 NEW: only one heart loss per object
    [HideInInspector] public bool hasPenalizedThisObject = false;

    void Start()
    {
        // wait for firstRayDelay, then spawn first ray
    }

    void Update()
    {
        // FIRST RAY
        if (!firstRaySpawned)
        {
            firstRayDelay -= Time.deltaTime;

            if (firstRayDelay <= 0f)
            {
                SpawnFirstRay();
                firstRaySpawned = true;
            }
            return;
        }

        // Normal spawning afterwards
        if (activeObject == null)
            SpawnObject();
    }

    void SpawnFirstRay()
    {
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;

        Vector3 spawnPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * spawnRadius;

        activeObject = Instantiate(sunrayPrefab, spawnPos, Quaternion.identity);

        Sunray ray = activeObject.GetComponent<Sunray>();
        Vector3 direction = (-spawnPos).normalized;

        ray.direction = direction;
        ray.speed = firstRaySpeed;
        ray.spawner = this;

        hasPenalizedThisObject = false; // reset penalty budget
    }

    public void SpawnObject()
    {
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;

        Vector3 spawnPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * spawnRadius;

        bool spawnPollution = Random.value < pollutionChance;

        if (spawnPollution)
        {
            activeObject = Instantiate(pollutionPrefab, spawnPos, Quaternion.identity);

            Pollution p = activeObject.GetComponent<Pollution>();
            Vector3 direction = (-spawnPos).normalized;

            p.direction = direction;
            p.speed = speedStages[Random.Range(0, speedStages.Length)];
            p.spawner = this;
        }
        else
        {
            activeObject = Instantiate(sunrayPrefab, spawnPos, Quaternion.identity);

            Sunray ray = activeObject.GetComponent<Sunray>();
            Vector3 direction = (-spawnPos).normalized;

            ray.direction = direction;
            ray.speed = speedStages[Random.Range(0, speedStages.Length)];
            ray.spawner = this;
        }

        hasPenalizedThisObject = false; // new object, fresh mistake budget
    }

    public void ReleaseObject()
    {
        activeObject = null;
        hasPenalizedThisObject = false;
    }
}
