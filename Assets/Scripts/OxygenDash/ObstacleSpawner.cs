using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnInterval = 1.2f;

    public float stopSpawningAt = 26f;   // stop obstacles at last 4 seconds
    public float boostWallAt = 15f;      // when the “wall requiring boost” appears

    public float columnX = 10f;
    public float laneSpacing = 1.2f;

    private float timer = 0f;
    private float gameTimer = 0f;

    void Update()
    {
        gameTimer += Time.deltaTime;

        // Stop spawning near the end
        if (gameTimer >= stopSpawningAt) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0;

            if (Mathf.Abs(gameTimer - boostWallAt) < 0.6f)
            {
                SpawnBoostRequiredWall();
            }
            else
            {
                SpawnNormalWave();
            }
        }
    }

    void SpawnNormalWave()
    {
        int lanes = 7; // from -3 to +3
        int gapLane = Random.Range(0, lanes); // random gap
        int maybeSecondGap = (Random.value > 0.7f) ? Random.Range(0, lanes) : -1;

        for (int i = 0; i < lanes; i++)
        {
            if (i == gapLane || i == maybeSecondGap)
                continue; // keep gaps free

            SpawnObstacleInLane(i - 3);
        }
    }

    void SpawnBoostRequiredWall()
    {
        int lanes = 7;
        for (int i = 0; i < lanes; i++)
        {
            SpawnObstacleInLane(i - 3);
        }
    }

    void SpawnObstacleInLane(int lane)
    {
        float yPos = lane * laneSpacing;

        Vector3 spawnPos = new Vector3(columnX, yPos, 0);

        Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
    }
}
