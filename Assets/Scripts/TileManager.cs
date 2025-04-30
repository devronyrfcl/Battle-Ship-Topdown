using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;        // Assign your tile prefab here
    public Transform player;             // Reference to the player
    public int tileLength = 302;         // Length of a single tile
    private int tilesOnScreen = 2;       // Only 2 tiles should be active at a time

    private float spawnX = 0f;           // Where the next tile should spawn
    private List<GameObject> activeTiles = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < tilesOnScreen; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (player.position.x - tileLength > (spawnX - tilesOnScreen * tileLength))
        {
            SpawnTile();
            DeleteOldestTile();
        }
    }

    void SpawnTile()
    {
        GameObject tile = Instantiate(tilePrefab, new Vector3(spawnX, 0, 0), Quaternion.identity);
        activeTiles.Add(tile);
        spawnX += tileLength;
    }

    void DeleteOldestTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}
