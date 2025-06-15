using System;
using System.Collections.Generic;
using UnityEngine;

public static class BlockManager
{
    private static Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();
    private static BlockHealthMap _blockHealth = new();

    // 이벤트를 통한 느슨한 결합
    public static event Action<Vector3Int> OnBlockDamaged;
    public static event Action<Vector3Int> OnBlockBroken;

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

    public static void UnregisterChunk(Vector2Int coord)
    {
        _chunks.Remove(coord);
    }

    public static void PlaceBlock(Vector3Int worldPos, VoxelType type)
    {
        if (TryGetChunk(worldPos, out var chunk, out var local))
        {
            chunk.SetBlock(local, type);
            _blockHealth.SetHealth(worldPos, GetInitialHealth(type));
        }
    }

    public static void DamageBlock(Vector3Int worldPos, int dmg, bool rebuildMesh = true)
    {
        if (!GetBlockType(worldPos, out var type) || type == VoxelType.Air)
            return;

        if (!_blockHealth.HasHealth(worldPos))
            _blockHealth.SetHealth(worldPos, GetInitialHealth(type));

        // 이벤트 발생으로 이펙트 시스템과 분리
        OnBlockDamaged?.Invoke(worldPos);

        if (_blockHealth.Damage(worldPos, dmg))
        {
            OnBlockBroken?.Invoke(worldPos);

            if (TryGetChunk(worldPos, out var chunk, out var local))
            {
                chunk.SetBlock(local, VoxelType.Air);
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
            type = chunk.GetBlock(localPos);
            return true;
        }
        type = VoxelType.Air;
        return false;
    }

}
