using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{
    private static Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();

    private static readonly Dictionary<VoxelType, int> _initialHealthMap = new Dictionary<VoxelType, int>
    {
        { VoxelType.Grass, 2 },
        { VoxelType.Dirt, 3 },
        { VoxelType.Stone, 6 }
    };

    public static void RegisterChunk(Vector2Int coord, Chunk chunk)
    {
        _chunks[coord] = chunk;
    }

    // 블럭 설치
    public static void PlaceBlock(Vector3Int worldPos, VoxelType type)
    {
        if (TryGetChunk(worldPos, out var chunk, out var local))
        {
            chunk.Blocks[local.x, local.y, local.z] = type;
            chunk.BuildMesh();
            BlockHealthController.SetHealth(worldPos, GetInitialHealth(type));
        }
    }

    // 블럭 데미지
    public static void DamageBlock(Vector3Int worldPos, int dmg)
    {
        if (!GetBlockType(worldPos, out var type) || type == VoxelType.Air)
            return;

        // 초기 체력 설정 (청크 생성 시 초기화되지 않은 경우)
        if (!BlockHealthController.HasHealth(worldPos))
        {
            BlockHealthController.SetHealth(worldPos, GetInitialHealth(type));
        }

        // 현재 HP 디버그 로그
        int preHp = BlockHealthController.GetHealth(worldPos);
        Debug.Log($"[BlockSystem] DamageBlock at {worldPos}, HP: {preHp} -> {preHp - dmg}");

        // 데미지 파티클 재생
        if (BlockEffectManager.Instance != null)
            BlockEffectManager.Instance.PlayDamageEffect(worldPos);

        // 체력 감소 및 파괴 처리
        if (BlockHealthController.Damage(worldPos, dmg))
        {
            // 파괴 파티클 재생
            if (BlockEffectManager.Instance != null)
                BlockEffectManager.Instance.PlayBreakEffect(worldPos);

            Debug.Log($"[BlockSystem] Block broken at {worldPos}");
            // 블럭 제거
            PlaceBlock(worldPos, VoxelType.Air);
        }
    }

    // 블럭 타입 조회
    public static bool GetBlockType(Vector3Int worldPos, out VoxelType type)
    {
        if (TryGetChunk(worldPos, out var chunk, out var local))
        {
            type = chunk.Blocks[local.x, local.y, local.z];
            return true;
        }
        type = VoxelType.Air;
        return false;
    }

    // 좌표 변환
    private static bool TryGetChunk(Vector3Int worldPos, out Chunk chunk, out Vector3Int localPos)
    {
        int chunkX = Mathf.FloorToInt(worldPos.x / (float)Chunk.CHUNK_WIDTH);
        int chunkZ = Mathf.FloorToInt(worldPos.z / (float)Chunk.CHUNK_WIDTH);
        var coord = new Vector2Int(chunkX, chunkZ);

        if (!_chunks.TryGetValue(coord, out chunk))
        {
            localPos = default;
            return false;
        }

        int localX = worldPos.x - coord.x * Chunk.CHUNK_WIDTH + 1;
        int localY = worldPos.y;
        int localZ = worldPos.z - coord.y * Chunk.CHUNK_WIDTH + 1;

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

    private static int GetInitialHealth(VoxelType type)
    {
        return _initialHealthMap.TryGetValue(type, out var hp) ? hp : 1;
    }
}
