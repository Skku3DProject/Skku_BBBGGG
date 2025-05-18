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

    // �� ��ġ
    public static void PlaceBlock(Vector3Int worldPos, VoxelType type)
    {
        if (TryGetChunk(worldPos, out var chunk, out var local))
        {
            chunk.Blocks[local.x, local.y, local.z] = type;
            chunk.BuildMesh();
            BlockHealthController.SetHealth(worldPos, GetInitialHealth(type));
        }
    }

    // �� ������
    public static void DamageBlock(Vector3Int worldPos, int dmg)
    {
        if (!GetBlockType(worldPos, out var type) || type == VoxelType.Air)
            return;

        // �ʱ� ü�� ���� (ûũ ���� �� �ʱ�ȭ���� ���� ���)
        if (!BlockHealthController.HasHealth(worldPos))
        {
            BlockHealthController.SetHealth(worldPos, GetInitialHealth(type));
        }

        // ���� HP ����� �α�
        int preHp = BlockHealthController.GetHealth(worldPos);
        Debug.Log($"[BlockSystem] DamageBlock at {worldPos}, HP: {preHp} -> {preHp - dmg}");

        // ������ ��ƼŬ ���
        if (BlockEffectManager.Instance != null)
            BlockEffectManager.Instance.PlayDamageEffect(worldPos);

        // ü�� ���� �� �ı� ó��
        if (BlockHealthController.Damage(worldPos, dmg))
        {
            // �ı� ��ƼŬ ���
            if (BlockEffectManager.Instance != null)
                BlockEffectManager.Instance.PlayBreakEffect(worldPos);

            Debug.Log($"[BlockSystem] Block broken at {worldPos}");
            // �� ����
            PlaceBlock(worldPos, VoxelType.Air);
        }
    }

    // �� Ÿ�� ��ȸ
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

    // ��ǥ ��ȯ
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
