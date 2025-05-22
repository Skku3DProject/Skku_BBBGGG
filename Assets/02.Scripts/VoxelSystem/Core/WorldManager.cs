using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("참조")]
    public GameObject ChunkPrefab;
    public Transform PlayerTransform;
    public Transform BaseCampTransform;
    public Transform SpawenrTransform;

    [Header("그리드 세팅")]
    public int GridWidth = 5;
    public int GridHeight = 5;
    public LayerMask ChunkLayer;

    [Header("스포너 배치 세팅")]
    public float Distance = 50f;


    [Header("식물 프리팹, 배치 설정")]
    public GameObject GrassPrefab;
    public GameObject MushroomPrefab;
    public GameObject TreePrefab;
    public GameObject IronstonePrefab;
    [Range(0f, 1f)] public float GrassDensity = 0.2f;
    [Range(0f, 1f)] public float MusroomDensity = 0.2f;
    [Range(0f, 1f)] public float TreeDensity = 0.05f;
    [Range(0f, 1f)] public float IronstoneDensity = 0.05f;

    [Header("지형 노이즈")]
    public float NoiseScale = 0.1f;

    private Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GenerateGrid();
        PositionPlayerAtCenter();
        PositionSpawner();
    }
    public void GenerateInEditor()
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

//        PopulateBlocksPerilnNoise(chunk, coord);
        PopulateBlocksCustomNoise(chunk, coord);

        chunk.BuildMesh();
        SpawnEnvironmentObjects(chunk);

       // BlockController.RegisterChunk(coord, chunk);
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
        BaseCampTransform.position = spawnPos;
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

    private void PopulateBlocksPerilnNoise(Chunk chunk, Vector2Int coord)
    {
        for (int x = 1; x <= Chunk.CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= Chunk.CHUNK_WIDTH; z++)
            {
                int globalX = coord.x * Chunk.CHUNK_WIDTH + (x - 1);
                int globalZ = coord.y * Chunk.CHUNK_WIDTH + (z - 1);

                // 1. 전체적으로 평탄한 기본 지형
                float baseNoise = Mathf.PerlinNoise(globalX * NoiseScale, globalZ * NoiseScale);
                float baseHeight = baseNoise * 1.5f + 5f; // 약간의 기복 + 기본 높이 상승

                // 2. 부드러운 언덕
                float hillNoise = Mathf.PerlinNoise(globalX * 0.02f + 1000f, globalZ * 0.02f + 1000f);
                float hillHeight = Mathf.SmoothStep(0f, 1f, hillNoise) * 4f;

                // 3. 산 노이즈 (확률 약간 증가 + 높이 강화)
                float mountainNoise = Mathf.PerlinNoise(globalX * 0.01f + 2345f, globalZ * 0.01f + 6789f);
                float mountainMask = Mathf.SmoothStep(0.85f, 0.92f, mountainNoise); // 조금 더 자주 나오게
                float mountainHeight = mountainMask * 20f; // 강한 고도 차이

                // 4. 최종 높이 계산
                float totalHeight = baseHeight + hillHeight + mountainHeight;
                int maxHeight = Mathf.Clamp(Mathf.FloorToInt(totalHeight), 0, Chunk.CHUNK_HEIGHT - 1);

                // 5. 블록 배치
                for (int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
                {
                    if (y > maxHeight)
                    {
                        chunk.Blocks[x, y, z] = VoxelType.Air;
                    }
                    else if (y == maxHeight)
                    {
                        chunk.Blocks[x, y, z] = maxHeight >= 20 ? VoxelType.Stone : VoxelType.Grass;
                    }
                    else if (y >= maxHeight - 3)
                    {
                        chunk.Blocks[x, y, z] = VoxelType.Dirt;
                    }
                    else
                    {
                        chunk.Blocks[x, y, z] = VoxelType.Stone;
                    }
                }
            }
        }
    }
    private void PopulateBlocksCustomNoise(Chunk chunk, Vector2Int coord)
    {
        for (int x = 1; x <= Chunk.CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= Chunk.CHUNK_WIDTH; z++)
            {
                int globalX = coord.x * Chunk.CHUNK_WIDTH + (x - 1);
                int globalZ = coord.y * Chunk.CHUNK_WIDTH + (z - 1);

                // ==== 1. 평지 베이스 ====
                float baseNoise = Mathf.PerlinNoise(globalX * NoiseScale, globalZ * NoiseScale);
                float baseHeight = baseNoise * 2f + 2f; // 낮고 완만한 평야

                // ==== 2. 언덕 노이즈 (높고 부드러운 변화) ====
                float hillNoise = Mathf.PerlinNoise(globalX * 0.03f + 2000f, globalZ * 0.03f + 2000f);
                float hillHeight = Mathf.SmoothStep(0f, 1f, hillNoise) * 8f;

                // ==== 3. 언덕이 나올 위치 마스킹 ====
                float hillMask = Mathf.PerlinNoise(globalX * 0.01f + 1234f, globalZ * 0.01f + 5678f);
                hillMask = Mathf.SmoothStep(0.5f, 0.7f, hillMask); // 언덕 확률 조절

                // ==== 4. 중심 평탄화 보정 ====
                float mapCenterX = GridWidth * Chunk.CHUNK_WIDTH / 2f;
                float mapCenterZ = GridHeight * Chunk.CHUNK_WIDTH / 2f;
                float distToCenter = Vector2.Distance(new Vector2(globalX, globalZ), new Vector2(mapCenterX, mapCenterZ));

                float flatRadius = 20f;
                float fadeRadius = 10f;
                float flatness = 0f;

                if (distToCenter < flatRadius)
                    flatness = 1f;
                else if (distToCenter < flatRadius + fadeRadius)
                    flatness = Mathf.SmoothStep(1f, 0f, (distToCenter - flatRadius) / fadeRadius);

                // ==== 5. 최종 높이 조합 ====
                float totalHeight = baseHeight + hillHeight * hillMask;
                float finalHeight = Mathf.Lerp(totalHeight, 4f, flatness); // 중심부는 평탄하게
                int maxHeight = Mathf.Clamp(Mathf.FloorToInt(finalHeight), 0, Chunk.CHUNK_HEIGHT - 1);

                // ==== 6. 블럭 채움 ====
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
    private void SpawnEnvironmentObjects(Chunk chunk)
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
                else if (r < GrassDensity + TreeDensity + MusroomDensity)
                {
                    Instantiate(MushroomPrefab, basePos, Quaternion.identity, chunk.transform);
                }
                else if (r < GrassDensity + TreeDensity + MusroomDensity + IronstoneDensity)
                {
                    Instantiate(IronstonePrefab, basePos, Quaternion.identity, chunk.transform);
                }
            }
        }
    }


}