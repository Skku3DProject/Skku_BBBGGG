using UnityEngine;
using System;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("식물 프리팹")]
    public GameObject GrassPrefab;
    public GameObject MushroomPrefab;
    public GameObject TreePrefab;
    public GameObject IronstonePrefab;

    [Header("배치 밀도")]
    [Range(0f, 1f)] public float GrassDensity = 0.2f;
    [Range(0f, 1f)] public float MushroomDensity = 0.2f;
    [Range(0f, 1f)] public float TreeDensity = 0.05f;
    [Range(0f, 1f)] public float IronstoneDensity = 0.05f;

    [Header("스폰 제외 설정")]
    public float CampBlockRadius = 15f;

    public static EnvironmentSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // WorldManager의 이벤트 구독
        if (WorldManager.instance != null)
        {
            //WorldManager.instance.OnChunkCreated += HandleChunkCreated;
            VoxelEvents.OnChunkCreated += HandleChunkCreated;
        }
    }

    private void Start()
    {
        // Start에서도 한번 더 확인 (WorldManager가 늦게 초기화될 경우 대비)
        if (WorldManager.instance != null)
        {
            //WorldManager.instance.OnChunkCreated -= HandleChunkCreated; // 중복 구독 방지
            //WorldManager.instance.OnChunkCreated += HandleChunkCreated;
            VoxelEvents.OnChunkCreated -= HandleChunkCreated;
            VoxelEvents.OnChunkCreated += HandleChunkCreated;
        }
    }

    private void OnDestroy()
    {
        if (WorldManager.instance != null)
        {
            //WorldManager.instance.OnChunkCreated -= HandleChunkCreated;
            VoxelEvents.OnChunkCreated -= HandleChunkCreated;
        }
    }

    private void HandleChunkCreated(Chunk chunk, Vector2 worldCenter)
    {
        SpawnEnvironmentObjects(chunk, worldCenter);
    }

    public void SpawnEnvironmentObjects(Chunk chunk, Vector2 worldCenter)
    {
        for (int x = 1; x <= Chunk.CHUNK_WIDTH; x++)
        {
            for (int z = 1; z <= Chunk.CHUNK_WIDTH; z++)
            {
                if (ShouldSkipObjectSpawn(chunk, x, z, worldCenter))
                    continue;

                SpawnRandomEnvironmentObject(chunk, x, z);
            }
        }
    }

    private bool ShouldSkipObjectSpawn(Chunk chunk, int x, int z, Vector2 worldCenter)
    {
        int surfaceY = FindSurfaceY(chunk, x, z);
        Vector3 worldPos = chunk.transform.position + new Vector3(x - 1 + 0.5f, surfaceY + 1.001f, z - 1 + 0.5f);

        float distToCenter = Vector2.Distance(new Vector2(worldPos.x, worldPos.z), worldCenter);
        return distToCenter < CampBlockRadius;
    }

    private void SpawnRandomEnvironmentObject(Chunk chunk, int x, int z)
    {
        int surfaceY = FindSurfaceY(chunk, x, z);
        Vector3 spawnPos = chunk.transform.position + new Vector3(x - 1 + 0.5f, surfaceY + 1.001f, z - 1 + 0.5f);

        float randomValue = UnityEngine.Random.value;

        if (randomValue < GrassDensity && GrassPrefab != null)
        {
            SpawnObject(GrassPrefab, spawnPos, GetRandomYRotation(), chunk.transform);
        }
        else if (randomValue < GrassDensity + TreeDensity && TreePrefab != null)
        {
            SpawnObject(TreePrefab, spawnPos, Quaternion.identity, chunk.transform);
        }
        else if (randomValue < GrassDensity + TreeDensity + MushroomDensity && MushroomPrefab != null)
        {
            SpawnObject(MushroomPrefab, spawnPos, Quaternion.identity, chunk.transform);
        }
        else if (randomValue < GrassDensity + TreeDensity + MushroomDensity + IronstoneDensity && IronstonePrefab != null)
        {
            SpawnObject(IronstonePrefab, spawnPos, Quaternion.identity, chunk.transform);
        }
    }

    private void SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        Instantiate(prefab, position, rotation, parent);
    }

    private Quaternion GetRandomYRotation()
    {
        return Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
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
}
