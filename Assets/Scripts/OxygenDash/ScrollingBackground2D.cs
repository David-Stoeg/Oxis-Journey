using UnityEngine;

public class ScrollingBackground2D : MonoBehaviour
{
    [Header("Tiles (add as many as you want)")]
    public Transform[] tiles;

    [Header("Scroll")]
    public float speed = 2f;       // units per second
    public bool scrollLeft = true;

    [Header("Auto width (recommended)")]
    public SpriteRenderer referenceRenderer; // drag ONE tile's SpriteRenderer here

    private float _tileWidth;

    void Start()
    {
        if (tiles == null || tiles.Length < 2)
        {
            Debug.LogWarning("ScrollingBackground2D: Assign at least 2 tiles.");
            return;
        }

        if (referenceRenderer != null)
            _tileWidth = referenceRenderer.bounds.size.x;
        else
            _tileWidth = Mathf.Abs(tiles[1].position.x - tiles[0].position.x); // fallback

        // Optional: make sure tiles are ordered left->right by x
        System.Array.Sort(tiles, (a, b) => a.position.x.CompareTo(b.position.x));
    }

    void Update()
    {
        if (tiles == null || tiles.Length < 2) return;

        float dir = scrollLeft ? -1f : 1f;
        float dx = speed * dir * Time.deltaTime;

        // Move all tiles
        for (int i = 0; i < tiles.Length; i++)
            tiles[i].position += new Vector3(dx, 0f, 0f);

        // Find leftmost and rightmost tiles
        Transform leftmost = tiles[0];
        Transform rightmost = tiles[0];

        for (int i = 1; i < tiles.Length; i++)
        {
            if (tiles[i].position.x < leftmost.position.x) leftmost = tiles[i];
            if (tiles[i].position.x > rightmost.position.x) rightmost = tiles[i];
        }

        // Wrap any tile that moved beyond the chain
        if (scrollLeft)
        {
            // If a tile is too far left, move it to the right end
            float cutoff = leftmost.position.x - _tileWidth;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].position.x < cutoff)
                {
                    tiles[i].position = new Vector3(rightmost.position.x + _tileWidth, tiles[i].position.y, tiles[i].position.z);
                    rightmost = tiles[i]; // update rightmost so multiple wraps work same frame
                }
            }
        }
        else
        {
            // scrolling right: if a tile is too far right, move it to the left end
            float cutoff = rightmost.position.x + _tileWidth;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].position.x > cutoff)
                {
                    tiles[i].position = new Vector3(leftmost.position.x - _tileWidth, tiles[i].position.y, tiles[i].position.z);
                    leftmost = tiles[i];
                }
            }
        }
    }
}
