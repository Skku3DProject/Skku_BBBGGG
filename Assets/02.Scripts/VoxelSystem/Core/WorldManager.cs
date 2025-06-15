using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[System.Serializable]
//public class SavedChunkData
//{
//    public VoxelType[,,] Blocks;

//    public SavedChunkData(VoxelType[,,] original)
//    {
//        int sizeX = original.GetLength(0);
//        int sizeY = original.GetLength(1);
//        int sizeZ = original.GetLength(2);
//        Blocks = new VoxelType[sizeX, sizeY, sizeZ];

//        for (int x = 0; x < sizeX; x++)
//            for (int y = 0; y < sizeY; y++)
//                for (int z = 0; z < sizeZ; z++)
//                    Blocks[x, y, z] = original[x, y, z];
//    }
//}

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    [Header("참조")]
    public GameObject ChunkPrefab;
    public GameObject DeadZonePrefab;
    public LayerMask ChunkLayer;

    [Header("그리드 세팅")]
    public int GridWidth = 5;
    public int GridHeight = 5;

    [Header("스포너 배치")]
    public float Distance = 50f;

    private readonly Dictionary<Vector2Int, Chunk> _chunks = new();
    private const int YIELD_INTERVAL = 10;

    // 이벤트
    public event Action OnCreatedWorld;
    public event Action OnResetWorld;
    public event Action<Chunk, Vector2> OnChunkCreated;
    public event Action<Vector3> OnWorldCenterReady;
    public event Action<Vector3> OnCreateEnemySpawenr;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //씬로드없이 실행할때 주석제거
        //InitializeWorld();
    }

    private void InitializeWorld()
    {
        int seed = UnityEngine.Random.Range(0, int.MaxValue);
        TerrainGenerator.Instance?.SetWorldSeed(seed);

        GenerateGrid();
        CreateDeadZone();
    }

    public void RegisterStageEventsForSceneLoad()
    {
        Debug.Log("씬로드 끝");
        CreateDeadZone();
        SetupWorldPositions();
    }

    public IEnumerator GenerateGridAsync(System.Action<float> onProgressUpdated = null)
    {
        int totalChunks = GridWidth * GridHeight;
        int createdChunks = 0;

        TerrainGenerator.Instance?.ClearHillPositions();

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

        // 보물상자 배치
        var hillPositions = TerrainGenerator.Instance?.GetHillPositions();
        if (hillPositions != null)
        {
            ChestSpawner.Instance?.PlaceChests(hillPositions, transform);
        }
        onProgressUpdated?.Invoke(1f);
    }

    private void GenerateGrid()
    {
        TerrainGenerator.Instance?.ClearHillPositions();

        for (int x = 0; x < GridWidth; x++)
        {
            for (int z = 0; z < GridHeight; z++)
            {
                CreateChunk(new Vector2Int(x, z));
            }
        }

        // 보물상자 배치
        var hillPositions = TerrainGenerator.Instance?.GetHillPositions();
        if (hillPositions != null)
        {
            ChestSpawner.Instance?.PlaceChests(hillPositions, transform);
        }
    }

    public IEnumerator InitWorldAsync(System.Action<float> onProgressUpdated)
    {
        yield return GenerateGridAsync(onProgressUpdated);
    }

    private void CreateChunk(Vector2Int coord)
    {
        Vector3 worldPos = new Vector3(coord.x * Chunk.CHUNK_WIDTH, 0f, coord.y * Chunk.CHUNK_WIDTH);
        GameObject chunkObj = Instantiate(ChunkPrefab, worldPos, Quaternion.identity, transform);
        Chunk chunk = chunkObj.GetComponent<Chunk>();

        // 지형 생성은 TerrainGenerator에 위임
        TerrainGenerator.Instance?.PopulateChunkTerrain(chunk, coord, GridWidth, GridHeight);
        chunk.BuildMesh();

        // 환경 오브젝트 스폰은 이벤트로 처리
        Vector2 worldCenter = GetWorldCenter();
        OnChunkCreated?.Invoke(chunk, worldCenter);

        _chunks.Add(coord, chunk);
    }

    public bool TryGetChunk(Vector2Int coord, out Chunk chunk)
    {
        return _chunks.TryGetValue(coord, out chunk);
    }

    public Dictionary<Vector2Int, Chunk> GetAllChunks()
    {
        return new Dictionary<Vector2Int, Chunk>(_chunks);
    }

    private Vector2 GetWorldCenter()
    {
        return new Vector2(
            GridWidth * Chunk.CHUNK_WIDTH / 2f,
            GridHeight * Chunk.CHUNK_WIDTH / 2f
        );
    }

    private Vector3 GetWorldCenter3D()
    {
        float centerX = GridWidth * Chunk.CHUNK_WIDTH / 2f;
        float centerZ = GridHeight * Chunk.CHUNK_WIDTH / 2f;

        // 월드 좌표를 청크 좌표로 변환
        int chunkX = Mathf.FloorToInt(centerX / Chunk.CHUNK_WIDTH);
        int chunkZ = Mathf.FloorToInt(centerZ / Chunk.CHUNK_WIDTH);
        Vector2Int chunkCoord = new Vector2Int(chunkX, chunkZ);

        if (!_chunks.TryGetValue(chunkCoord, out Chunk chunk))
            return new Vector3(centerX, 10f, centerZ);

        // 청크 내 로컬 좌표 계산
        int localX = Mathf.FloorToInt(centerX - chunkX * Chunk.CHUNK_WIDTH) + 1;
        int localZ = Mathf.FloorToInt(centerZ - chunkZ * Chunk.CHUNK_WIDTH) + 1;

        // 경계 체크
        localX = Mathf.Clamp(localX, 1, Chunk.CHUNK_WIDTH);
        localZ = Mathf.Clamp(localZ, 1, Chunk.CHUNK_WIDTH);

        float surfaceY = TerrainGenerator.Instance?.FindSurfaceY(chunk, localX, localZ) ?? 10f;
        return new Vector3(centerX, surfaceY, centerZ);
    }

    private void SetupWorldPositions()
    {
        if (_chunks.Count == 0) return;

        Vector3 centerPos = GetWorldCenter3D();
        Vector3 spawnerPos = CalculateSpawnerPosition(centerPos);

        Debug.Log($"월드 중심 위치: {centerPos}, 스포너 위치: {spawnerPos}");

        // 이벤트 발생
        OnWorldCenterReady?.Invoke(centerPos);
        OnCreateEnemySpawenr?.Invoke(spawnerPos);
    }

    private Vector3 CalculateSpawnerPosition(Vector3 centerPos)
    {
        Vector3 targetPos = centerPos + Vector3.forward * Distance;

        int spawnerX = Mathf.FloorToInt(targetPos.x);
        int spawnerZ = Mathf.FloorToInt(targetPos.z);

        Vector2Int spawnerCoord = new Vector2Int(spawnerX / Chunk.CHUNK_WIDTH, spawnerZ / Chunk.CHUNK_WIDTH);

        if (!_chunks.TryGetValue(spawnerCoord, out Chunk spawnerChunk))
            return targetPos;

        int localX = (spawnerX % Chunk.CHUNK_WIDTH) + 1;
        int localZ = (spawnerZ % Chunk.CHUNK_WIDTH) + 1;
        int surfaceY = TerrainGenerator.Instance?.FindSurfaceY(spawnerChunk, localX, localZ) ?? 0;

        return new Vector3(spawnerX + 0.5f, surfaceY + 2f, spawnerZ + 0.5f);
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