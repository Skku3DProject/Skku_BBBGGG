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
    public static void DamageBlock(Vector3Int worldPos, int dmg, bool rebuildMesh = true)
    {

        if (!GetBlockType(worldPos, out var type) || type == VoxelType.Air)
            return;

        if (!BlockHealthController.HasHealth(worldPos))
            BlockHealthController.SetHealth(worldPos, GetInitialHealth(type));

        int preHp = BlockHealthController.GetHealth(worldPos);
        Debug.Log($"[BlockSystem] DamageBlock at {worldPos}, HP: {preHp} -> {preHp - dmg}");

        BlockEffectManager.Instance?.PlayDamageEffect(worldPos);

        if (BlockHealthController.Damage(worldPos, dmg))
        {
            BlockEffectManager.Instance?.PlayBreakEffect(worldPos);
            Debug.Log($"[BlockSystem] Block broken at {worldPos}");

            if (TryGetChunk(worldPos, out var chunk, out var local))
            {
                chunk.Blocks[local.x, local.y, local.z] = VoxelType.Air;
                if (rebuildMesh)
                    chunk.BuildMesh();
            }
        }
        else if (rebuildMesh)
        {
            if (TryGetChunk(worldPos, out var chunk, out _))
                chunk.BuildMesh();
        }
    }

    public static void DamageBlocksInRadius(Vector3 center, float radius, int damage)
    {
        HashSet<Chunk> affectedChunks = new HashSet<Chunk>();

        int minX = Mathf.FloorToInt(center.x - radius);
        int maxX = Mathf.CeilToInt(center.x + radius);
        int minY = Mathf.FloorToInt(center.y - radius);
        int maxY = Mathf.CeilToInt(center.y + radius);
        int minZ = Mathf.FloorToInt(center.z - radius);
        int maxZ = Mathf.CeilToInt(center.z + radius);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (Vector3.Distance(center, pos + Vector3.one * 0.5f) <= radius)
                    {
                        if (TryGetChunk(pos, out var chunk, out _))
                        {
                            DamageBlock(pos, damage, rebuildMesh: false);
                            affectedChunks.Add(chunk);
                        }
                    }
                }
            }
        }

        foreach (var chunk in affectedChunks)
        {
            chunk.BuildMesh();
        }
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
}
