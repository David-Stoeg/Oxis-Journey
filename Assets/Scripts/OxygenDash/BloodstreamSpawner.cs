using UnityEngine;

public class BloodstreamSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bacteriaPrefab;
    public GameObject hemoglobinPrefab;
    public GameObject endCellPrefab;

    [Header("Timing")]
    public float spawnInterval = 1.2f;
    public float hemoglobinSpawnTime = 12f;
    public float boostWallTime = 14f;
    public float stopSpawningAt = 26f;
    public float endCellSpawnTime = 27f;

    [Header("Lanes")]
    public int lanes = 7;          // -3 to +3
    public float laneSpacing = 1.2f;

    private float timer = 0f;
    private float gameTimer = 0f;
    private bool hemoglobinSpawned = false;
    private bool wallSpawned = false;
    private bool endCellSpawned = false;

    void Update()
    {
        gameTimer += Time.deltaTime;

        // 1) Spawn hemoglobin ONCE
        if (!hemoglobinSpawned && gameTimer >= hemoglobinSpawnTime)
        {
            hemoglobinSpawned = true;
            SpawnHemoglobin();
            Debug.Log("Hemoglobin spawned!");
        }

        // 2) Spawn full wall ONCE
        if (!wallSpawned && gameTimer >= boostWallTime)
        {
            wallSpawned = true;
            SpawnWallWithoutGap();
            Debug.Log("BOOST WALL SPAWNED!");
        }

        // 3) Stop obstacle waves at X seconds
        if (gameTimer >= stopSpawningAt)
        {
            if (!endCellSpawned)
            {
                endCellSpawned = true;
                SpawnEndCell();
                Debug.Log("End Cell Spawned!");
            }
            return;
        }

        // 4) Normal wave spawning
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0;
            SpawnWaveWithGap();
        }
    }

    void SpawnWaveWithGap()
    {
        int gapLane = Random.Range(0, lanes);
        int? secondGap = (Random.value > 0.7f) ? Random.Range(0, lanes) : (int?)null;

        for (int i = 0; i < lanes; i++)
        {
            if (i == gapLane) continue;
            if (secondGap != null && i == secondGap.Value) continue;

            SpawnBacteriaInLane(i - lanes / 2);
        }
    }

    void SpawnWallWithoutGap()
    {
        for (int i = 0; i < lanes; i++)
        {
            SpawnBacteriaInLane(i - lanes / 2);
        }
    }

    void SpawnHemoglobin()
    {
        if (hemoglobinPrefab == null)
        {
            Debug.LogError("Hemoglobin prefab is NOT assigned!");
            return;
        }

        float startX = Camera.main.transform.position.x + 12f;
        Vector3 pos = new Vector3(startX, 0, 0);

        Debug.Log("Spawning Hemoglobin at: " + pos);
        Instantiate(hemoglobinPrefab, pos, Quaternion.identity);
    }

    void SpawnEndCell()
    {
        if (endCellPrefab == null)
        {
            Debug.LogError("END CELL prefab is NOT assigned!");
            return;
        }

        float startX = Camera.main.transform.position.x + 14f;
        Vector3 pos = new Vector3(startX, 0, 0);

        Debug.Log("Spawning END CELL at: " + pos);
        Instantiate(endCellPrefab, pos, Quaternion.identity);
    }

    void SpawnBacteriaInLane(int lane)
    {
        float y = lane * laneSpacing;

        Vector3 pos = new Vector3(Camera.main.transform.position.x + 12f, y, 0);
        GameObject obj = Instantiate(bacteriaPrefab, pos, Quaternion.identity);

        // Make sure obstacle only uses MoveLeft
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }
    }
}
