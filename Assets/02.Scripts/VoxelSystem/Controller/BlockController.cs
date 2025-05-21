using System.Collections.Generic;
using UnityEngine;


//사용안하는 코드... 이전꺼임
public class BlockController : MonoBehaviour
{
    [Header("참조")]
    public Camera PlayerCamera;            // 클릭에 사용할 카메라. 없으면 Transform.forward 사용
    public LayerMask ChunkLayer;           // 청크 Collider 만 포함된 레이어
    public float MaxDistance = 4f;         // Raycast 최대 거리

    [Header("복셀 세팅")]
    public VoxelType PlaceType = VoxelType.Grass;  // 설치할 블럭 타입

    // 청크 좌표 → Chunk 인스턴스 맵
    private static Dictionary<Vector2Int, Chunk> _registeredChunks = new Dictionary<Vector2Int, Chunk>();
    private static Dictionary<Vector3Int, int> _blockHealthMap = new(); // 위치 → 체력
    private static readonly Dictionary<VoxelType, int> _initialHealthMap = new()
{
    { VoxelType.Grass, 2 },
    { VoxelType.Dirt, 3 },
    { VoxelType.Stone, 6 }
};
    void Update()
    {
        HandleBlockPlacement();
    }
    private bool HandleBlockPlacement()
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (!leftClick && !rightClick) return false;

        Ray ray = PlayerCamera != null ? PlayerCamera.ScreenPointToRay(Input.mousePosition) : new Ray(transform.position, transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, MaxDistance, ChunkLayer))
            return false;

        Vector3Int blockPos = GetAdjustedBlockPosition(hit, placing: rightClick);

        if (rightClick) // 우클릭 → 설치
        {
            if (TryGetLocalPosition(blockPos, out Chunk chunk, out Vector3Int localPos))
            {
                chunk.Blocks[localPos.x, localPos.y, localPos.z] = PlaceType;
                chunk.BuildMesh();
                _blockHealthMap[blockPos] = GetInitialHealth(blockPos); // 설치 시 체력도 세팅
                return true;
            }
        }
        else if (leftClick) // 좌클릭 → 데미지
        {
            BlockController.DamageBlock(blockPos, 1);
            return true;
        }

        return false;
    }

    private Vector3Int GetAdjustedBlockPosition(RaycastHit hit, bool placing)
    {
        float offset = placing ? 0.5f : -0.5f;
        Vector3 adjusted = hit.point + hit.normal * offset;
        return Vector3Int.FloorToInt(adjusted);
    }

    private bool TryGetLocalPosition(Vector3Int blockPos, out Chunk chunk, out Vector3Int localPos)
    {
        int chunkX = Mathf.FloorToInt(blockPos.x / (float)Chunk.CHUNK_WIDTH);
        int chunkZ = Mathf.FloorToInt(blockPos.z / (float)Chunk.CHUNK_WIDTH);
        var coord = new Vector2Int(chunkX, chunkZ);

        if (!_registeredChunks.TryGetValue(coord, out chunk))
        {
            localPos = default;
            return false;
        }

        int localX = blockPos.x - chunkX * Chunk.CHUNK_WIDTH + 1;
        int localY = blockPos.y;
        int localZ = blockPos.z - chunkZ * Chunk.CHUNK_WIDTH + 1;

        bool valid = localX >= 0 && localX < Chunk.CHUNK_WIDTH + 2
                  && localY >= 0 && localY < Chunk.CHUNK_HEIGHT
                  && localZ >= 0 && localZ < Chunk.CHUNK_WIDTH + 2;

        if (!valid)
        {
            localPos = default;
            return false;
        }

        localPos = new Vector3Int(localX, localY, localZ);
        return true;
    }

    public static bool GetBlockType(Vector3Int worldPos, out VoxelType type)
    {
        if (TryGetChunk(worldPos, out var chunk, out var localPos))
        {
            type = chunk.Blocks[localPos.x, localPos.y, localPos.z];
            return true;
        }
        type = VoxelType.Air;
        return false;
    }

    public static bool SetBlockType(Vector3Int worldPos, VoxelType newType)
    {
        if (TryGetChunk(worldPos, out var chunk, out var localPos))
        {
            chunk.Blocks[localPos.x, localPos.y, localPos.z] = newType;
            chunk.BuildMesh();
            return true;
        }
        return false;
    }

    private static bool TryGetChunk(Vector3Int blockPos, out Chunk chunk, out Vector3Int localPos)
    {
        int chunkX = Mathf.FloorToInt(blockPos.x / (float)Chunk.CHUNK_WIDTH);
        int chunkZ = Mathf.FloorToInt(blockPos.z / (float)Chunk.CHUNK_WIDTH);
        var coord = new Vector2Int(chunkX, chunkZ);

        if (!_registeredChunks.TryGetValue(coord, out chunk))
        {
            localPos = default;
            return false;
        }

        int localX = blockPos.x - chunkX * Chunk.CHUNK_WIDTH + 1;
        int localY = blockPos.y;
        int localZ = blockPos.z - chunkZ * Chunk.CHUNK_WIDTH + 1;

        bool valid = localX >= 0 && localX < Chunk.CHUNK_WIDTH + 2
                  && localY >= 0 && localY < Chunk.CHUNK_HEIGHT
                  && localZ >= 0 && localZ < Chunk.CHUNK_WIDTH + 2;

        if (!valid)
        {
            localPos = default;
            return false;
        }

        localPos = new Vector3Int(localX, localY, localZ);
        return true;
    }

    public static int GetBlockHealth(Vector3Int pos)
    {
        return _blockHealthMap.TryGetValue(pos, out var hp) ? hp : GetInitialHealth(pos);
    }

    public static void DamageBlock(Vector3Int pos, int damage)
    {
        if (!GetBlockType(pos, out var type) || type == VoxelType.Air)
            return;

        if (!_blockHealthMap.ContainsKey(pos))
            _blockHealthMap[pos] = GetInitialHealth(pos);

        _blockHealthMap[pos] -= damage;
        if (_blockHealthMap[pos] <= 0)
        {
            SetBlockType(pos, VoxelType.Air);
            _blockHealthMap.Remove(pos);
        }
    }

    private static int GetInitialHealth(Vector3Int pos)
    {
        if (!GetBlockType(pos, out var type))
            return 1;

        return _initialHealthMap.TryGetValue(type, out var hp) ? hp : 1;
    }

    // WorldManager.CreateChunk 에서 호출해서 맵에 등록
    public static void RegisterChunk(Vector2Int coord, Chunk chunk)
    {
        _registeredChunks[coord] = chunk;
    }

    public static void ClearRegistry()
    {
        _registeredChunks.Clear();
    }
}
