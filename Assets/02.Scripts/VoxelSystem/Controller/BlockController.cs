using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("����")]
    public Camera PlayerCamera;            // Ŭ���� ����� ī�޶�. ������ Transform.forward ���
    public LayerMask ChunkLayer;           // ûũ Collider �� ���Ե� ���̾�
    public float MaxDistance = 4f;         // Raycast �ִ� �Ÿ�

    [Header("���� ����")]
    public VoxelType PlaceType = VoxelType.Grass;  // ��ġ�� �� Ÿ��

    // ûũ ��ǥ �� Chunk �ν��Ͻ� ��
    private static Dictionary<Vector2Int, Chunk> _registeredChunks = new Dictionary<Vector2Int, Chunk>();

    void Update()
    {
        if (BuildModeManager.Instance != null && BuildModeManager.Instance.IsBuildMode)
            return;

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

        bool placing = leftClick;
        Vector3Int blockPos = GetAdjustedBlockPosition(hit, placing);

        if (TryGetLocalPosition(blockPos, out Chunk chunk, out Vector3Int localPos))
        {
            chunk.Blocks[localPos.x, localPos.y, localPos.z] = placing ? PlaceType : VoxelType.Air;
            chunk.BuildMesh();
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
    // WorldManager.CreateChunk ���� ȣ���ؼ� �ʿ� ���
    public static void RegisterChunk(Vector2Int coord, Chunk chunk)
    {
        _registeredChunks[coord] = chunk;
    }

    public static void ClearRegistry()
    {
        _registeredChunks.Clear();
    }
}
