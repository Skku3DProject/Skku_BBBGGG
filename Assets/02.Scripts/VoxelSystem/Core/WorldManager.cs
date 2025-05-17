using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("References")]
    public GameObject chunkPrefab;
    public Transform playerTransform;


    [Tooltip("Number of chunks along X (width)")]
    public int gridWidth = 5;
    [Tooltip("Number of chunks along Z (height)")]
    public int gridHeight = 5;

    [Header("Vegetation Prefabs & Settings")]
    public GameObject grassPrefab;
    public GameObject treePrefab;
    [Range(0f, 1f)] public float grassDensity = 0.2f;
    [Range(0f, 1f)] public float treeDensity = 0.05f;

    [Header("Noise Settings")]
    public float noiseScale = 0.1f;

    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    void Start()
    {
        GenerateGrid();
        PositionPlayerAtCenter();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
            for (int z = 0; z < gridHeight; z++)
                CreateChunk(new Vector2Int(x, z));
    }

    void CreateChunk(Vector2Int coord)
    {
        Vector3 worldPos = new Vector3(coord.x * Chunk.chunkWidth, 0, coord.y * Chunk.chunkWidth);
        GameObject obj = Instantiate(chunkPrefab, worldPos, Quaternion.identity, transform);
        Chunk chunk = obj.GetComponent<Chunk>();

        PopulateBlocks(chunk, coord);
        chunk.BuildMesh();
        SpawnVegetation(chunk);

        BlockController.RegisterChunk(coord, chunk);

        chunks.Add(coord, chunk);
    }
    void PositionPlayerAtCenter()
    {
        if (playerTransform == null || chunks.Count == 0)
            return;

        int worldWidth = gridWidth * Chunk.chunkWidth;
        int worldDepth = gridHeight * Chunk.chunkWidth;
        int centerX = worldWidth / 2;
        int centerZ = worldDepth / 2;

        // Determine chunk containing center
        int chunkX = centerX / Chunk.chunkWidth;
        int chunkZ = centerZ / Chunk.chunkWidth;
        Vector2Int centerCoord = new Vector2Int(chunkX, chunkZ);

        if (!chunks.TryGetValue(centerCoord, out Chunk centerChunk))
            return;

        // Local indices within chunk (1 to chunkWidth)
        int localX = (centerX % Chunk.chunkWidth) + 1;
        int localZ = (centerZ % Chunk.chunkWidth) + 1;

        // Find highest grass block at center cell
        int surfaceY = 0;
        for (int y = Chunk.chunkHeight - 1; y >= 0; y--)
        {
            if (centerChunk.blocks[localX, y, localZ] == VoxelType.Grass)
            {
                surfaceY = y;
                break;
            }
        }

        // Set player position just above surface
        Vector3 spawnPos = new Vector3(centerX + 0.5f, surfaceY + 2.0f, centerZ + 0.5f);
        playerTransform.position = spawnPos;
    }
    void PopulateBlocks(Chunk chunk, Vector2Int coord)
    {
        for (int x = 1; x <= Chunk.chunkWidth; x++)
            for (int z = 1; z <= Chunk.chunkWidth; z++)
            {
                int gx = coord.x * Chunk.chunkWidth + (x - 1);
                int gz = coord.y * Chunk.chunkWidth + (z - 1);
                float n = Mathf.PerlinNoise(gx * noiseScale, gz * noiseScale);
                int height = Mathf.FloorToInt(n * (Chunk.chunkHeight - 1));

                for (int y = 0; y < Chunk.chunkHeight; y++)
                    chunk.blocks[x, y, z] =
                        y <= height
                            ? y == height ? VoxelType.Grass : y >= height - 3 ? VoxelType.Dirt : VoxelType.Stone
                            : VoxelType.Air;
            }
    }

    void SpawnVegetation(Chunk chunk)
    {
        for (int x = 1; x <= Chunk.chunkWidth; x++)
            for (int z = 1; z <= Chunk.chunkWidth; z++)
            {
                int y = 0;
                for (int yy = Chunk.chunkHeight - 1; yy >= 0; yy--)
                    if (chunk.blocks[x, yy, z] == VoxelType.Grass)
                    {
                        y = yy;
                        break;
                    }

                float r = Random.value;
                Vector3 basePos = chunk.transform.position + new Vector3(x - 1 + 0.5f, y + 1.001f, z - 1 + 0.5f);
                if (r < grassDensity)
                    Instantiate(grassPrefab, basePos, Quaternion.Euler(0, Random.Range(0, 360), 0), chunk.transform);
                else if (r < grassDensity + treeDensity)
                    Instantiate(treePrefab, basePos, Quaternion.identity, chunk.transform);
            }
    }
}