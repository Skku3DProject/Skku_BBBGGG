using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("참조")]
    public GameObject ChunkPrefab;
    public Transform PlayerTransform;
    public Transform SpawenrTransform;

    [Header("그리드 세팅")]
    public int GridWidth = 5;
    public int GridHeight = 5;
    public LayerMask ChunkLayer;

    [Header("스포너 배치 세팅")]
    public float Distance = 50f;


    [Header("식물 프리팹, 배치 설정")]
    public GameObject GrassPrefab;
    public GameObject TreePrefab;
    [Range(0f, 1f)] public float GrassDensity = 0.2f;
    [Range(0f, 1f)] public float TreeDensity = 0.05f;

    [Header("지형 노이즈")]
    public float NoiseScale = 0.1f;

    private Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();

    void Start()
    {
        GenerateGrid();
        PositionPlayerAtCenter();
        PositionSpawner();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int z = 0; z < GridHeight; z++)
            {
                CreateChunk(new Vector2Int(x, z));
            }
        }
    }

    void CreateChunk(Vector2Int coord)
    {
        Vector3 worldPos = new Vector3(coord.x * Chunk.CHUNK_WIDTH, 0f, coord.y * Chunk.CHUNK_WIDTH);
        GameObject chunkObj = Instantiate(ChunkPrefab, worldPos, Quaternion.identity, transform);
        Chunk chunk = chunkObj.GetComponent<Chunk>();

        PopulateBlocks(chunk, coord);
        chunk.BuildMesh();
        SpawnVegetation(chunk);

        BlockController.RegisterChunk(coord, chunk);
        _chunks.Add(coord, chunk);
    }

    void PositionPlayerAtCenter()
    {
        if (PlayerTransform == null || _chunks.Count == 0)
            return;

        int totalWidth = GridWidth * Chunk.CHUNK_WIDTH;
        int totalDepth = GridHeight * Chunk.CHUNK_WIDTH;
        int centerX = totalWidth / 2;
        int centerZ = totalDepth / 2;

        Vector2Int centerCoord = new Vector2Int(centerX / Chunk.CHUNK_WIDTH, centerZ / Chunk.CHUNK_WIDTH);
        if (!_chunks.TryGetValue(centerCoord, out Chunk centerChunk))
            return;

        int localX = (centerX % Chunk.CHUNK_WIDTH) + 1;
        int localZ = (centerZ % Chunk.CHUNK_WIDTH) + 1;

        int surfaceY = FindSurfaceY(centerChunk, localX, localZ);
        Vector3 spawnPos = new Vector3(centerX + 0.5f, surfaceY + 2f, centerZ + 0.5f);
        PlayerTransform.position = spawnPos;
    }
    void PositionSpawner()
    {
        if (PlayerTransform == null || _chunks.Count == 0)
            return;

        // 플레이어 기준 중심 좌표 계산
        int totalWidth = GridWidth * Chunk.CHUNK_WIDTH;
        int totalDepth = GridHeight * Chunk.CHUNK_WIDTH;
        int centerX = totalWidth / 2;
        int centerZ = totalDepth / 2;

        Vector2Int centerCoord = new Vector2Int(centerX / Chunk.CHUNK_WIDTH, centerZ / Chunk.CHUNK_WIDTH);
        if (!_chunks.TryGetValue(centerCoord, out Chunk centerChunk))
            return;

        int localX = (centerX % Chunk.CHUNK_WIDTH) + 1;
        int localZ = (centerZ % Chunk.CHUNK_WIDTH) + 1;

        int surfaceY = FindSurfaceY(centerChunk, localX, localZ);

        Vector3 playerPos = PlayerTransform.position;
        Vector3 forward = PlayerTransform.forward;
        Vector3 frontPos = playerPos + forward.normalized * Distance;

        int spawnerX = Mathf.FloorToInt(frontPos.x);
        int spawnerZ = Mathf.FloorToInt(frontPos.z);

        Vector2Int frontCoord = new Vector2Int(spawnerX / Chunk.CHUNK_WIDTH, spawnerZ / Chunk.CHUNK_WIDTH);
        if (!_chunks.TryGetValue(frontCoord, out Chunk frontChunk))
            return;

        int localFrontX = (spawnerX % Chunk.CHUNK_WIDTH) + 1;
        int localFrontZ = (spawnerZ % Chunk.CHUNK_WIDTH) + 1;
        int frontSurfaceY = FindSurfaceY(frontChunk, localFrontX, localFrontZ);

        Vector3 spawnPos = new Vector3(spawnerX + 0.5f, frontSurfaceY + 2f, spawnerZ + 0.5f);
        SpawenrTransform.position = spawnPos;
    }

    private int FindSurfaceY(Chunk chunk, int x, int z)
    {
        for (int y = Chunk.CHUNK_HEIGHT - 1; y >= 0; y--)
        {
            if (chunk.Blocks[x, y, z] == VoxelType.Grass)
                return y;
        }
        return 0;
    }

    private void PopulateBlocks(Chunk chunk, Vector2Int coord)
    {
        for (int x = 1; x <= Chunk.CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= Chunk.CHUNK_WIDTH; z++)
            {
                int globalX = coord.x * Chunk.CHUNK_WIDTH + (x - 1);
                int globalZ = coord.y * Chunk.CHUNK_WIDTH + (z - 1);
                float noiseValue = Mathf.PerlinNoise(globalX * NoiseScale, globalZ * NoiseScale);
                int maxHeight = Mathf.FloorToInt(noiseValue * (Chunk.CHUNK_HEIGHT - 1));

                for (int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
                {
                    chunk.Blocks[x, y, z] = y <= maxHeight
                        ? (y == maxHeight
                            ? VoxelType.Grass
                            : (y >= maxHeight - 3
                                ? VoxelType.Dirt
                                : VoxelType.Stone))
                        : VoxelType.Air;
                }
            }
        }
    }
    private void SpawnVegetation(Chunk chunk)
    {
        for (int x = 1; x <= Chunk.CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= Chunk.CHUNK_WIDTH; z++)
            {
                int surfaceY = FindSurfaceY(chunk, x, z);
                Vector3 basePos = chunk.transform.position + new Vector3(x - 1 + 0.5f, surfaceY + 1.001f, z - 1 + 0.5f);

                float r = Random.value;
                if (r < GrassDensity)
                {
                    Instantiate(GrassPrefab, basePos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), chunk.transform);
                }
                else if (r < GrassDensity + TreeDensity)
                {
                    Instantiate(TreePrefab, basePos, Quaternion.identity, chunk.transform);
                }
            }
        }
    }
}