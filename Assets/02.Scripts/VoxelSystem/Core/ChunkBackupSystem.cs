using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SavedChunkData
{
    public VoxelType[,,] Blocks;

    public SavedChunkData(VoxelType[,,] original)
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
public class ChunkBackupSystem : MonoBehaviour
{
    public static ChunkBackupSystem Instance { get; private set; }

    [Header("백업 설정")]
    public int BackupRadius = 6;

    private readonly Dictionary<Vector2Int, SavedChunkData> _backupData = new();
    private Vector2Int _backupCenter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // StageManager 이벤트 구독
        if (StageManager.instance != null)
        {
            StageManager.instance.OnCombatStart += BackupCentralChunks;
            StageManager.instance.OnCombatEnd += RestoreCentralChunks;
            Debug.Log("ChunkBackupSystem이 StageManager 이벤트에 구독");
        }
    }

    private void OnDestroy()
    {
        if (StageManager.instance != null)
        {
            StageManager.instance.OnCombatStart -= BackupCentralChunks;
            StageManager.instance.OnCombatEnd -= RestoreCentralChunks;
        }
    }

    public void BackupCentralChunks()
    {
        if (WorldManager.instance == null) return;

        _backupData.Clear();

        // 월드 중심 계산
        int centerX = WorldManager.instance.GridWidth / 2;
        int centerZ = WorldManager.instance.GridHeight / 2;
        _backupCenter = new Vector2Int(centerX, centerZ);

        Debug.Log($"청크 백업 시작 - 중심: {_backupCenter}, 반경: {BackupRadius}");

        int backedUpCount = 0;
        for (int dx = -BackupRadius; dx <= BackupRadius; dx++)
        {
            for (int dz = -BackupRadius; dz <= BackupRadius; dz++)
            {
                Vector2Int coord = new Vector2Int(_backupCenter.x + dx, _backupCenter.y + dz);
                if (WorldManager.instance.TryGetChunk(coord, out var chunk))
                {
                    _backupData[coord] = new SavedChunkData(chunk.Blocks);
                    backedUpCount++;
                }
            }
        }

        Debug.Log($"총 {backedUpCount}개 청크 백업 완료");
    }

    public void RestoreCentralChunks()
    {
        Debug.Log("청크 복원 시작");

        int restoredCount = 0;
        foreach (var kvp in _backupData)
        {
            Vector2Int coord = kvp.Key;
            if (WorldManager.instance != null && WorldManager.instance.TryGetChunk(coord, out var chunk))
            {
                RestoreChunkBlocks(chunk, kvp.Value);
                chunk.BuildMesh();
                restoredCount++;
            }
        }

        Debug.Log($"총 {restoredCount}개 청크 복원 완료");
    }

    public void ClearBackupData()
    {
        _backupData.Clear();
        Debug.Log("백업 데이터 클리어");
    }

    public bool HasBackupData()
    {
        return _backupData.Count > 0;
    }

    public int GetBackupCount()
    {
        return _backupData.Count;
    }

    private void RestoreChunkBlocks(Chunk chunk, SavedChunkData savedData)
    {
        int sizeX = savedData.Blocks.GetLength(0);
        int sizeY = savedData.Blocks.GetLength(1);
        int sizeZ = savedData.Blocks.GetLength(2);

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    chunk.Blocks[x, y, z] = savedData.Blocks[x, y, z];
    }
}
