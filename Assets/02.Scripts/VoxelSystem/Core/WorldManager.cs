using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class TempChunkData
{
    public VoxelType[,,] Blocks;

    public TempChunkData(VoxelType[,,] original)
    {
        int sizeX = original.GetLength(0);
        int sizeY = original.GetLength(1);
        int sizeZ = original.GetLength(2);
        Blocks = new VoxelType[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    Blocks[x, y, z] = original[x, y, z];
    }
}

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    [Header("참조")]
    public GameObject ChunkPrefab;
    public GameObject Player;
    public Vector3 StartPos;
    public Transform BaseCampTransform;
    public Transform SpawenrTransform;
    public GameObject DeadZonePrefab;
    public LayerMask ChunkLayer;

    [Header("보물상자")]
    public GameObject ChestPrefab;
    public int MaxChestCount = 10;

    [Header("그리드 세팅")]
    public int GridWidth = 5;
    public int GridHeight = 5;

    [Header("스포너 배치")]
    public float Distance = 50f;

    [Header("지형 노이즈")]
    public float NoiseScale = 0.1f;

    // 환경 오브젝트 관련 필드들 제거됨

    private int _worldSeed = 0;
    private float _randomOffsetX;
    private float _randomOffsetZ;

    private readonly Dictionary<Vector2Int, TempChunkData> _savedChunkData = new();
    private readonly Dictionary<Vector2Int, Chunk> _chunks = new();
    private readonly List<Vector3> _hillPositions = new();

    private Vector2Int _centerCoord;
    private const int SAVE_RADIUS = 6;
    private const float FLAT_RADIUS = 48f;
    private const float FADE_RADIUS = 32f;
    private const float HEIGHT_THRESHOLD = 18f;
    private const int YIELD_INTERVAL = 10;

    // 이벤트 추가
    public event Action OnCreatedWorld;
    public event Action OnResetWorld;
    public event Action<Chunk, Vector2> OnChunkCreated; // 새로 추가

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //씬로드없이 실행할때 주석제거
        //RegisterStageEvents();
        //SetupCursor();
        //InitializeWorld();
    }

    private void RegisterStageEvents()
    {
        if (StageManager.instance != null)
        {
            StageManager.instance.OnCombatStart += BackupCentralChunks;
            StageManager.instance.OnCombatEnd += RestoreCentralChunks;
            StageManager.instance.OnCombatEnd += ResetPlayer;
        }
    }

    private void SetupCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitializeWorld()
    {
        SetWorldSeed(UnityEngine.Random.Range(0, int.MaxValue));
        BackupCentralChunks();
        GenerateGrid();
        PositionPlayerAtCenter();
        PositionSpawner();
        CreateDeadZone();
    }

    public void InitWorld()
    {
        SetWorldSeed(UnityEngine.Random.Range(0, int.MaxValue));
        BackupCentralChunks();
        GenerateGrid();
    }

    public void RegisterStageEventsForSceneLoad()
    {
        if (StageManager.instance != null)
        {
            Debug.Log("씬로드 끝");
            RegisterStageEvents();
            UpdateSceneReferences();
            SetupCursor();
            CreateDeadZone();
        }
    }

    public void GenerateInEditor()
    {
        GenerateGrid();
        PositionPlayerAtCenter();
        PositionSpawner();
    }

    public void SetWorldSeed(int seed)
    {
        _worldSeed = seed;
        System.Random rnd = new System.Random(_worldSeed);
        _randomOffsetX = (float)rnd.NextDouble() * 10000f;
        _randomOffsetZ = (float)rnd.NextDouble() * 10000f;
    }

    public IEnumerator GenerateGridAsync(System.Action<float> onProgressUpdated = null)
    {
        int totalChunks = GridWidth * GridHeight;
        int createdChunks = 0;

        _hillPositions.Clear();

        for (int x = 0; x < GridWidth; x++)
        {
            for (int z = 0; z < GridHeight; z++)
            {
                CreateChunk(new Vector2Int(x, z));
                createdChunks++;

                float progress = (float)createdChunks / totalChunks;
                onProgressUpdated?.Invoke(progress);

                if (createdChunks % YIELD_INTERVAL == 0)
                    yield return null;
            }
        }

        PlaceChests();
        onProgressUpdated?.Invoke(1f);
    }

    public IEnumerator InitWorldAsync(System.Action<float> onProgressUpdated)
    {
        BackupCentralChunks();
        yield return GenerateGridAsync(onProgressUpdated);
    }

    private void GenerateGrid()
    {
        _hillPositions.Clear();

        for (int x = 0; x < GridWidth; x++)
        {
            for (int z = 0; z < GridHeight; z++)
            {
                CreateChunk(new Vector2Int(x, z));
            }
        }

        PlaceChests();
    }

    private void CreateChunk(Vector2Int coord)
    {
        Vector3 worldPos = new Vector3(coord.x * Chunk.CHUNK_WIDTH, 0f, coord.y * Chunk.CHUNK_WIDTH);
        GameObject chunkObj = Instantiate(ChunkPrefab, worldPos, Quaternion.identity, transform);
        Chunk chunk = chunkObj.GetComponent<Chunk>();

        PopulateChunkTerrain(chunk, coord);
        chunk.BuildMesh();

        // 환경 오브젝트 스폰은 이벤트로 처리
        Vector2 worldCenter = GetWorldCenter();
        OnChunkCreated?.Invoke(chunk, worldCenter);

        _chunks.Add(coord, chunk);
    }

    private Vector2 GetWorldCenter()
    {
        return new Vector2(
            GridWidth * Chunk.CHUNK_WIDTH / 2f,
            GridHeight * Chunk.CHUNK_WIDTH / 2f
        );
    }

    // 환경 오브젝트 관련 메서드들 제거됨 (EnvironmentSpawner로 이동)

    private void PopulateChunkTerrain(Chunk chunk, Vector2Int coord)
    {
        for (int x = 1; x <= Chunk.CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= Chunk.CHUNK_WIDTH; z++)
            {
                GenerateTerrainColumn(chunk, coord, x, z);
            }
        }
    }

    private void GenerateTerrainColumn(Chunk chunk, Vector2Int coord, int x, int z)
    {
        int globalX = coord.x * Chunk.CHUNK_WIDTH + (x - 1);
        int globalZ = coord.y * Chunk.CHUNK_WIDTH + (z - 1);

        int maxHeight = CalculateTerrainHeight(globalX, globalZ);

        for (int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
        {
            chunk.Blocks[x, y, z] = DetermineBlockType(y, maxHeight, globalX, globalZ);
        }

        // 언덕 위치 기록
        if (maxHeight >= HEIGHT_THRESHOLD)
        {
            Vector3 hillPos = chunk.transform.position + new Vector3(x - 1 + 0.5f, maxHeight + 1f, z - 1 + 0.5f);
            _hillPositions.Add(hillPos);
        }
    }

    private int CalculateTerrainHeight(int globalX, int globalZ)
    {
        // 평지 베이스
        float baseNoise = Mathf.PerlinNoise(globalX * NoiseScale, globalZ * NoiseScale);
        float baseHeight = baseNoise * 4f + 6f;

        // 언덕 노이즈
        float hillNoise = Mathf.PerlinNoise(globalX * 0.03f + 2000f, globalZ * 0.03f + 2000f);
        float hillHeight = Mathf.SmoothStep(0f, 1f, hillNoise) * 25f;

        // 언덕 마스크
        float hillMask = Mathf.PerlinNoise(globalX * 0.01f + 1234f, globalZ * 0.01f + 5678f);
        hillMask = Mathf.SmoothStep(0.4f, 0.65f, hillMask);

        // 중심 평탄화
        float flatness = CalculateCenterFlatness(globalX, globalZ);

        float totalHeight = baseHeight + hillHeight * hillMask;
        float targetHeight = Mathf.Lerp(totalHeight, 10f, flatness);

        return Mathf.Clamp(Mathf.FloorToInt(targetHeight), 0, Chunk.CHUNK_HEIGHT - 1);
    }

    private float CalculateCenterFlatness(int globalX, int globalZ)
    {
        float mapCenterX = GridWidth * Chunk.CHUNK_WIDTH / 2f;
        float mapCenterZ = GridHeight * Chunk.CHUNK_WIDTH / 2f;
        float distToCenter = Vector2.Distance(new Vector2(globalX, globalZ), new Vector2(mapCenterX, mapCenterZ));

        if (distToCenter < FLAT_RADIUS)
            return 1f;
        else if (distToCenter < FLAT_RADIUS + FADE_RADIUS)
            return Mathf.SmoothStep(1f, 0f, (distToCenter - FLAT_RADIUS) / FADE_RADIUS);

        return 0f;
    }

    private VoxelType DetermineBlockType(int y, int maxHeight, int globalX, int globalZ)
    {
        if (y > maxHeight)
            return VoxelType.Air;

        if (y == maxHeight)
        {
            // 중심에서 먼 위치는 snow, 가까우면 grass
            float distToCenter = Vector2.Distance(
                new Vector2(globalX, globalZ),
                new Vector2(GridWidth * Chunk.CHUNK_WIDTH / 2f, GridHeight * Chunk.CHUNK_WIDTH / 2f)
            );
            return distToCenter >= 240f ? VoxelType.Snow : VoxelType.Grass;
        }

        if (y >= maxHeight - 3)
            return VoxelType.Dirt;

        return VoxelType.Stone;
    }

    private void PlaceChests()
    {
        int chestCount = Mathf.Min(MaxChestCount, _hillPositions.Count);
        if (chestCount == 0) return;

        var shuffledPositions = new List<Vector3>(_hillPositions);
        ShuffleList(shuffledPositions);

        for (int i = 0; i < chestCount; i++)
        {
            Instantiate(ChestPrefab, shuffledPositions[i], Quaternion.identity, transform);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void PositionPlayerAtCenter()
    {
        if (Player == null || _chunks.Count == 0)
            return;

        var centerInfo = GetCenterChunkInfo();
        if (!centerInfo.HasValue)
            return;

        var (centerChunk, localX, localZ, worldX, worldZ) = centerInfo.Value;

        int surfaceY = FindSurfaceY(centerChunk, localX, localZ);
        Vector3 spawnPos = new Vector3(worldX + 0.5f, surfaceY + 2f, worldZ + 0.5f);

        Debug.Log("플레이어 위치: " + spawnPos);

        StartPos = spawnPos;
        Player.transform.position = spawnPos + new Vector3(0, 10, 0f);

        if (BaseCampTransform != null)
            BaseCampTransform.position = spawnPos;
    }

    private void PositionSpawner()
    {
        if (Player == null || SpawenrTransform == null || _chunks.Count == 0)
            return;

        Vector3 playerPos = Player.transform.position;
        Vector3 forward = Player.transform.forward;
        Vector3 targetPos = playerPos + forward.normalized * Distance;

        int spawnerX = Mathf.FloorToInt(targetPos.x);
        int spawnerZ = Mathf.FloorToInt(targetPos.z);

        Vector2Int spawnerCoord = new Vector2Int(spawnerX / Chunk.CHUNK_WIDTH, spawnerZ / Chunk.CHUNK_WIDTH);

        if (!_chunks.TryGetValue(spawnerCoord, out Chunk spawnerChunk))
            return;

        int localX = (spawnerX % Chunk.CHUNK_WIDTH) + 1;
        int localZ = (spawnerZ % Chunk.CHUNK_WIDTH) + 1;
        int surfaceY = FindSurfaceY(spawnerChunk, localX, localZ);

        Vector3 spawnPos = new Vector3(spawnerX + 0.5f, surfaceY + 2f, spawnerZ + 0.5f);
        SpawenrTransform.position = spawnPos;
    }

    private (Chunk chunk, int localX, int localZ, int worldX, int worldZ)? GetCenterChunkInfo()
    {
        int totalWidth = GridWidth * Chunk.CHUNK_WIDTH;
        int totalDepth = GridHeight * Chunk.CHUNK_WIDTH;
        int centerX = totalWidth / 2;
        int centerZ = totalDepth / 2;

        Vector2Int centerCoord = new Vector2Int(centerX / Chunk.CHUNK_WIDTH, centerZ / Chunk.CHUNK_WIDTH);

        if (!_chunks.TryGetValue(centerCoord, out Chunk centerChunk))
            return null;

        int localX = (centerX % Chunk.CHUNK_WIDTH) + 1;
        int localZ = (centerZ % Chunk.CHUNK_WIDTH) + 1;

        return (centerChunk, localX, localZ, centerX, centerZ);
    }

    private int FindSurfaceY(Chunk chunk, int x, int z)
    {
        for (int y = Chunk.CHUNK_HEIGHT - 1; y >= 0; y--)
        {
            VoxelType type = chunk.Blocks[x, y, z];
            if (type == VoxelType.Grass || type == VoxelType.Snow)
                return y;
        }
        return 0;
    }

    private void UpdateSceneReferences()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        BaseCampTransform = GameObject.FindGameObjectWithTag("BaseTower")?.transform;
        SpawenrTransform = GameObject.FindGameObjectWithTag("EnemySpawner")?.transform;

        PositionPlayerAtCenter();
        PositionSpawner();
    }

    private void ResetPlayer()
    {
        if (Player != null)
        {
            var playerController = Player.GetComponent<ThirdPersonPlayer>();
            if (playerController != null)
            {
                playerController.CharacterController.enabled = false;
                Player.transform.position = StartPos;
                playerController.CharacterController.enabled = true;
            }
        }
    }

    public void BackupCentralChunks()
    {
        _savedChunkData.Clear();

        int centerX = GridWidth / 2;
        int centerZ = GridHeight / 2;
        _centerCoord = new Vector2Int(centerX, centerZ);

        for (int dx = -SAVE_RADIUS; dx <= SAVE_RADIUS; dx++)
        {
            for (int dz = -SAVE_RADIUS; dz <= SAVE_RADIUS; dz++)
            {
                Vector2Int coord = new Vector2Int(_centerCoord.x + dx, _centerCoord.y + dz);
                if (_chunks.TryGetValue(coord, out var chunk))
                {
                    _savedChunkData[coord] = new TempChunkData(chunk.Blocks);
                }
            }
        }
    }

    public void RestoreCentralChunks()
    {
        Debug.Log("복원 시작");

        foreach (var kvp in _savedChunkData)
        {
            Vector2Int coord = kvp.Key;
            if (_chunks.TryGetValue(coord, out var chunk))
            {
                RestoreChunkBlocks(chunk, kvp.Value);
                chunk.BuildMesh();
            }
        }
    }

    private void RestoreChunkBlocks(Chunk chunk, TempChunkData savedData)
    {
        int sizeX = savedData.Blocks.GetLength(0);
        int sizeY = savedData.Blocks.GetLength(1);
        int sizeZ = savedData.Blocks.GetLength(2);

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    chunk.Blocks[x, y, z] = savedData.Blocks[x, y, z];
    }

    private void CreateDeadZone()
    {
        if (DeadZonePrefab == null)
        {
            Debug.LogWarning("DeadZonePrefab이 설정되지 않았습니다.");
            return;
        }

        float worldWidth = GridWidth * Chunk.CHUNK_WIDTH;
        float worldDepth = GridHeight * Chunk.CHUNK_WIDTH;

        Vector3 position = new Vector3(worldWidth / 2f, -50f, worldDepth / 2f);
        Vector3 scale = new Vector3(worldWidth, 10f, worldDepth);

        GameObject deadZone = Instantiate(DeadZonePrefab, position, Quaternion.identity, transform);
        deadZone.name = "DeadZone";
        deadZone.transform.localScale = scale;
    }
}