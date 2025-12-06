using UnityEngine;

public class BloodstreamSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bacteriaPrefab;
    public GameObject endCellPrefab;

    [Header("Timing")]
    public float spawnInterval = 1.0f;
    public float stopSpawningAt = 26f;
    public float finishDelay = 3f;   // ← NEW: extra delay before finish appears
    public float endCellOffset = 14f;

    [Header("Lanes")]
    public int lanes = 7;
    public float laneSpacing = 1.2f;
    public float laneJitter = 0.25f;

    [Header("Safe Corridor Settings")]
    [Range(1, 3)] public int corridorWidth = 2;

    [Header("Obstacle Randomness")]
    public float rotationMin = 0f;
    public float rotationMax = 360f;

    private float timer = 0f;
    private float gameTimer = 0f;
    private bool finishSpawned = false;
    
    [Header("Horizontal Offsets")]
    public float xJitter = 0.5f;

    void Update()
    {
        gameTimer += Time.deltaTime;

        // 1) Stop obstacles after stopSpawningAt
        if (gameTimer >= stopSpawningAt)
        {
            if (!finishSpawned && gameTimer >= stopSpawningAt + finishDelay)
            {
                SpawnEndCell();
                finishSpawned = true;
            }
            return;
        }

        // 2) Spawn normal waves
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        int center = Random.Range(0, lanes);
        int half = corridorWidth / 2;

        for (int i = 0; i < lanes; i++)
        {
            if (Mathf.Abs(i - center) <= half)
                continue;

            SpawnRandomizedBacteria(i - lanes / 2);
        }
    }

    void SpawnRandomizedBacteria(int lane)
    {
        // Vertical position with jitter
        float y = (lane * laneSpacing) + Random.Range(-laneJitter, laneJitter);

        // Horizontal jitter (slight left-right variance)
        float jitteredX = Camera.main.transform.position.x + 12f + Random.Range(-xJitter, xJitter);

        Vector3 pos = new Vector3(jitteredX, y, 0);

        GameObject obj = Instantiate(bacteriaPrefab, pos, Quaternion.identity);

        // Ensure correct tag
        obj.tag = "Obstacle";

        // Rotate ONLY the child sprite
        Transform spriteChild = obj.transform.Find("Sprite"); // or "Sprite" depending on your prefab
        if (spriteChild != null)
        {
            float zRotation = Random.Range(rotationMin, rotationMax);
            spriteChild.localRotation = Quaternion.Euler(0f, 0f, zRotation);
        }
        else
        {
            Debug.LogWarning("Bacteria prefab missing child named 'Sprite'!");
        }

        // Make sure physics is disabled
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }
    }

    void SpawnEndCell()
    {
        Vector3 pos = new Vector3(
            Camera.main.transform.position.x + endCellOffset,
            0,
            0
        );

        GameObject cell = Instantiate(endCellPrefab, pos, Quaternion.identity);
        cell.tag = "Finish"; // ensure recognition by player script
    }
}
