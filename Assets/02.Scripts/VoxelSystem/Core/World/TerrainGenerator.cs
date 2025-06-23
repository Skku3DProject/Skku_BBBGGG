using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TerrainSettings
{
    [Header("지형 노이즈")]
    public float NoiseScale = 0.1f;

    [Header("지형 설정")]
    public float FlatRadius = 48f;
    public float FadeRadius = 32f;
    public float HeightThreshold = 18f;

}

public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator Instance { get; private set; }

    [Header("지형 설정")]
    public TerrainSettings Settings;

    private int _worldSeed = 0;
    private float _randomOffsetX;
    private float _randomOffsetZ;
    private readonly List<Vector3> _hillPositions = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetWorldSeed(int seed)
    {
        _worldSeed = seed;
        System.Random rnd = new System.Random(_worldSeed);
        _randomOffsetX = (float)rnd.NextDouble() * 10000f;
        _randomOffsetZ = (float)rnd.NextDouble() * 10000f;
    }

    public void ClearHillPositions()
    {
        _hillPositions.Clear();
    }

    public List<Vector3> GetHillPositions()
    {
        return new List<Vector3>(_hillPositions);
    }

    public void PopulateChunkTerrain(Chunk chunk, Vector2Int coord, int gridWidth, int gridHeight)
    {
        for (int x = 1; x <= Chunk.CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= Chunk.CHUNK_WIDTH; z++)
            {
                GenerateTerrainColumn(chunk, coord, x, z, gridWidth, gridHeight);
            }
        }
    }

    private void GenerateTerrainColumn(Chunk chunk, Vector2Int coord, int x, int z, int gridWidth, int gridHeight)
    {
        int globalX = coord.x * Chunk.CHUNK_WIDTH + (x - 1);
        int globalZ = coord.y * Chunk.CHUNK_WIDTH + (z - 1);

        int maxHeight = CalculateTerrainHeight(globalX, globalZ, gridWidth, gridHeight);

        for (int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
        {
            chunk.Blocks[x, y, z] = DetermineBlockType(y, maxHeight, globalX, globalZ, gridWidth, gridHeight);
        }

        // 언덕 위치 기록
        if (maxHeight >= Settings.HeightThreshold)
        {
            Vector3 hillPos = chunk.transform.position + new Vector3(x - 1 + 0.5f, maxHeight + 1f, z - 1 + 0.5f);
            _hillPositions.Add(hillPos);
        }
    }

    private int CalculateTerrainHeight(int globalX, int globalZ, int gridWidth, int gridHeight)
    {
        // 평지 베이스
        float baseNoise = Mathf.PerlinNoise(globalX * Settings.NoiseScale, globalZ * Settings.NoiseScale);
        float baseHeight = baseNoise * 4f + 6f;

        // 언덕 노이즈
        float hillNoise = Mathf.PerlinNoise(globalX * 0.03f + 2000f, globalZ * 0.03f + 2000f);
        float hillHeight = Mathf.SmoothStep(0f, 1f, hillNoise) * 25f;

        // 언덕 마스크
        float hillMask = Mathf.PerlinNoise(globalX * 0.01f + 1234f, globalZ * 0.01f + 5678f);
        hillMask = Mathf.SmoothStep(0.4f, 0.65f, hillMask);

        // 중심 평탄화
        float flatness = CalculateCenterFlatness(globalX, globalZ, gridWidth, gridHeight);

        float totalHeight = baseHeight + hillHeight * hillMask;
        float targetHeight = Mathf.Lerp(totalHeight, 10f, flatness);

        return Mathf.Clamp(Mathf.FloorToInt(targetHeight), 0, Chunk.CHUNK_HEIGHT - 1);
    }

    private float CalculateCenterFlatness(int globalX, int globalZ, int gridWidth, int gridHeight)
    {
        float mapCenterX = gridWidth * Chunk.CHUNK_WIDTH / 2f;
        float mapCenterZ = gridHeight * Chunk.CHUNK_WIDTH / 2f;
        float distToCenter = Vector2.Distance(new Vector2(globalX, globalZ), new Vector2(mapCenterX, mapCenterZ));

        if (distToCenter < Settings.FlatRadius)
            return 1f;
        else if (distToCenter < Settings.FlatRadius + Settings.FadeRadius)
            return Mathf.SmoothStep(1f, 0f, (distToCenter - Settings.FlatRadius) / Settings.FadeRadius);

        return 0f;
    }

    private VoxelType DetermineBlockType(int y, int maxHeight, int globalX, int globalZ, int gridWidth, int gridHeight)
    {
        if (y > maxHeight)
            return VoxelType.Air;

        if (y == maxHeight)
        {
            // 중심에서 먼 위치는 snow, 가까우면 grass
            float distToCenter = Vector2.Distance(
                new Vector2(globalX, globalZ),
                new Vector2(gridWidth * Chunk.CHUNK_WIDTH / 2f, gridHeight * Chunk.CHUNK_WIDTH / 2f)
            );
            return distToCenter >= 240f ? VoxelType.Snow : VoxelType.Grass;
        }

        if (y >= maxHeight - 3)
            return VoxelType.Dirt;

        return VoxelType.Stone;
    }

    public int FindSurfaceY(Chunk chunk, int x, int z)
    {
        for (int y = Chunk.CHUNK_HEIGHT - 1; y >= 0; y--)
        {
            VoxelType type = chunk.Blocks[x, y, z];
            if (type == VoxelType.Grass || type == VoxelType.Snow)
                return y;
        }
        return 0;
    }
}
