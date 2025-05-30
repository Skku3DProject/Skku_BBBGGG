using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    [Header("참조")]
    public GameObject ChunkPrefab;
    public GameObject Player;
    public Vector3 StartPos;
    public Transform BaseCampTransform;
    public Transform SpawenrTransform;
    [Header("보물상자 프리팹")]
    public GameObject ChestPrefab;
    public int MaxChestCount = 10;
    private List<Vector3> _cavePositions = new(); // 동굴 후보 위치
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



    private Dictionary<Vector2Int, TempChunkData> _savedChunkData = new();
    private Vector2Int _centerCoord;
    private int _saveRadius = 2; // 예: 5x5 청크 저장

    private Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();

    void Start()
    {
        StageManager.instance.OnCombatStart += BackupCentralChunks;
        StageManager.instance.OnCombatEnd += RestoreCentralChunks;
        StageManager.instance.OnCombatEnd += ResetPlayer;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        BackupCentralChunks();
        GenerateGrid();
        PositionPlayerAtCenter();
        PositionSpawner();
    }

    //--------------------------------------- 맵로딩용
    public void InitWorld()
    {
        BackupCentralChunks();
        GenerateGrid();
    }
    public void RegisterStageEvents()
    {
        if (StageManager.instance != null)
        {
            Debug.Log("씬로드 끝");

            StageManager.instance.OnCombatStart += BackupCentralChunks;
            StageManager.instance.OnCombatEnd += RestoreCentralChunks;
            //StageManager.instance.OnCombatEnd += ResetPlayer;

            Player = GameObject.FindGameObjectWithTag("Player");
            BaseCampTransform = GameObject.FindGameObjectWithTag("BaseTower").transform;
            SpawenrTransform = GameObject.FindGameObjectWithTag("EnemySpawner").transform;
            PositionPlayerAtCenter();
            PositionSpawner();
            //StartCoroutine(DelayedPositioning());
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    //-----------------------------------맵 로딩 용

    public IEnumerator GenerateGridAsync(System.Action<float> onProgressUpdated = null)
    {
        int totalChunks = GridWidth * GridHeight;
        int createdChunks = 0;
        int yieldInterval = 10; // 5개마다 한 번씩 프레임 분산

        for (int x = 0; x < GridWidth; x++)
        {
            for (int z = 0; z < GridHeight; z++)
            {
                CreateChunk(new Vector2Int(x, z));
                createdChunks++;

                // 진행률 콜백
                float progress = (float)createdChunks / totalChunks;
                onProgressUpdated?.Invoke(progress);

                // 묶음 단위로만 yield
                if (createdChunks % yieldInterval == 0)
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

    private void ResetPlayer()
    {
        Player.transform.position = StartPos;
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

        PlaceChests(); // 생성 후 보물상자 배치
    }
    void PlaceChests()
    {
        int chestCount = Mathf.Min(MaxChestCount, _cavePositions.Count);
        if (chestCount == 0) return;

        List<Vector3> shuffled = new List<Vector3>(_cavePositions);
        ShuffleList(shuffled); // 랜덤 섞기

        for (int i = 0; i < chestCount; i++)
        {
            Instantiate(ChestPrefab, shuffled[i], Quaternion.identity, transform);
        }
    }
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
    void CreateChunk(Vector2Int coord)
    {
        Vector3 worldPos = new Vector3(coord.x * Chunk.CHUNK_WIDTH, 0f, coord.y * Chunk.CHUNK_WIDTH);
        GameObject chunkObj = Instantiate(ChunkPrefab, worldPos, Quaternion.identity, transform);
        Chunk chunk = chunkObj.GetComponent<Chunk>();

        PopulateBlocksPerilnNoise(chunk, coord);
        //PopulateBlocksCustomNoise(chunk, coord);

        chunk.BuildMesh();
        SpawnEnvironmentObjects(chunk);

        // BlockController.RegisterChunk(coord, chunk);
        _chunks.Add(coord, chunk);
    }
    //private IEnumerator DelayedPositioning()
    //{
    //    yield return new WaitForEndOfFrame();

    //    // 이제 안전하게 중앙에 배치
    //    PositionPlayerAtCenter();
    //    PositionSpawner();

    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;

    //    Debug.Log("플레이어, 스포너 재배치 완료");
    //}
    void PositionPlayerAtCenter()
    {
        if (Player == null || _chunks.Count == 0)
            return;

        int totalWidth = GridWidth * Chunk.CHUNK_WIDTH;
        int totalDepth = GridHeight * Chunk.CHUNK_WIDTH;
        int centerX = totalWidth / 2;
        int centerZ = totalDepth / 2;

        Vector2Int centerCoord = new Vector2Int(centerX / Chunk.CHUNK_WIDTH, centerZ / Chunk.CHUNK_WIDTH);
        if (!_chunks.TryGetValue(centerCoord, out Chunk centerChunk))
            return;

        Debug.Log("씬로드끝 플레이어 재배치");


        int localX = (centerX % Chunk.CHUNK_WIDTH) + 1;
        int localZ = (centerZ % Chunk.CHUNK_WIDTH) + 1;

        int surfaceY = FindSurfaceY(centerChunk, localX, localZ);
        Vector3 spawnPos = new Vector3(centerX + 0.5f, surfaceY + 2f, centerZ + 0.5f);

        Debug.Log(spawnPos);
        StartPos = spawnPos;
        Player.transform.position = spawnPos+new Vector3(0, 10,0f);
        BaseCampTransform.position = spawnPos;
    }
    void PositionSpawner()
    {
        if (Player == null || _chunks.Count == 0)
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

        Vector3 playerPos = Player.transform.position;
        Vector3 forward = Player.transform.forward;
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

                // ==== 1. 평지 베이스 ====
                float baseNoise = Mathf.PerlinNoise(globalX * NoiseScale, globalZ * NoiseScale);
                float baseHeight = baseNoise * 4f + 6f; // 2~4 사이의 낮은 평지

                // ==== 2. 언덕 노이즈 ====
                float hillNoise = Mathf.PerlinNoise(globalX * 0.03f + 2000f, globalZ * 0.03f + 2000f);
                float hillHeight = Mathf.SmoothStep(0f, 1f, hillNoise) * 25f; // 최대 15

                // ==== 3. 언덕 마스크 ====
                float hillMask = Mathf.PerlinNoise(globalX * 0.01f + 1234f, globalZ * 0.01f + 5678f);
                hillMask = Mathf.SmoothStep(0.4f, 0.65f, hillMask); // 언덕 위치 마스킹

                // ==== 4. 중심 평탄화 영역 설정 ====
                float mapCenterX = GridWidth * Chunk.CHUNK_WIDTH / 2f;
                float mapCenterZ = GridHeight * Chunk.CHUNK_WIDTH / 2f;
                float distToCenter = Vector2.Distance(new Vector2(globalX, globalZ), new Vector2(mapCenterX, mapCenterZ));

                float flatRadius = 48f; // 약 6청크 반지름
                float fadeRadius = 32f; // 경계 흐리기 범위
                float flatness = 0f;

                if (distToCenter < flatRadius)
                    flatness = 1f;
                else if (distToCenter < flatRadius + fadeRadius)
                    flatness = Mathf.SmoothStep(1f, 0f, (distToCenter - flatRadius) / fadeRadius);

                // ==== 5. 최종 높이 계산 ====
                float totalHeight = baseHeight + hillHeight * hillMask;
                float targetHeight = Mathf.Lerp(totalHeight, 10f, flatness); // 중심은 높이 10으로
                int maxHeight = Mathf.Clamp(Mathf.FloorToInt(targetHeight), 0, Chunk.CHUNK_HEIGHT - 1);

                // ==== 6. 동굴 노이즈 (단순 제거형 동굴) ====
                for (int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
                {
                    //float caveNoise = Mathf.PerlinNoise(globalX * 0.1f + 4000f, globalZ * 0.1f + 4000f + y * 0.05f);
                    //bool isCave = (y < maxHeight - 3 && caveNoise > 0.65f); // 듬성듬성 구멍

                    float caveNoise = CaveNoise3D(globalX * 0.02f, y * 0.02f, globalZ * 0.02f);
                    bool isCave = (y < maxHeight - 3 && caveNoise > 0.58f);

                    if (y > maxHeight || isCave)
                    {
                        chunk.Blocks[x, y, z] = VoxelType.Air;
                        if (y > 3 && y + 1 < Chunk.CHUNK_HEIGHT)
                        {
                            VoxelType below = chunk.Blocks[x, y - 1, z];
                            if (below == VoxelType.Stone || below == VoxelType.Dirt || below == VoxelType.Grass)
                            {
                                Vector3 pos = chunk.transform.position + new Vector3(x - 1 + 0.5f, y + 0.5f, z - 1 + 0.5f);
                                _cavePositions.Add(pos);
                            }
                        }
                    }
                    else if (y == maxHeight)
                    {
                        chunk.Blocks[x, y, z] = VoxelType.Grass;
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
    // 유사 3D 노이즈 함수
    float CaveNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);
        return (xy + yz + zx) / 3f;
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

    public void BackupCentralChunks()
    {
        _savedChunkData.Clear();

        int centerX = GridWidth / 2;
        int centerZ = GridHeight / 2;
        _centerCoord = new Vector2Int(centerX, centerZ);

        for (int dx = -_saveRadius; dx <= _saveRadius; dx++)
        {
            for (int dz = -_saveRadius; dz <= _saveRadius; dz++)
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
        Debug.Log("ReStoreStart");
        foreach (var pair in _savedChunkData)
        {
            Vector2Int coord = pair.Key;
            if (_chunks.TryGetValue(coord, out var chunk))
            {
                var savedData = pair.Value;
                int sizeX = savedData.Blocks.GetLength(0);
                int sizeY = savedData.Blocks.GetLength(1);
                int sizeZ = savedData.Blocks.GetLength(2);

                for (int x = 0; x < sizeX; x++)
                    for (int y = 0; y < sizeY; y++)
                        for (int z = 0; z < sizeZ; z++)
                            chunk.Blocks[x, y, z] = savedData.Blocks[x, y, z];

                chunk.BuildMesh(); // 메쉬 재생성
            }
        }
    }

}