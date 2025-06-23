using System;
using UnityEngine;

//�̺�Ʈ���� ���⿡ ���Ƴ���
public static class VoxelEvents
{
    // ��� ���� �̺�Ʈ
    public static event Action<Vector3Int> OnBlockDamaged;
    public static event Action<Vector3Int> OnBlockBroken;

    // ���� ���� �̺�Ʈ
    public static event Action OnWorldCreated;
    public static event Action OnWorldReset;
    public static event Action<Vector3> OnWorldCenterReady;
    public static event Action<Vector3> OnCreateEnemySpawenr;

    // ûũ ���� �̺�Ʈ
    public static event Action<Chunk, Vector2> OnChunkCreated;

    //Invoke �޼����
    public static void InvokeCreateEnemySpawner(Vector3 position)
    {
        OnCreateEnemySpawenr?.Invoke(position);
    }

    public static void InvokeBlockDamaged(Vector3Int position)
    {
        OnBlockDamaged?.Invoke(position);
    }

    public static void InvokeBlockBroken(Vector3Int position)
    {
        OnBlockBroken?.Invoke(position);
    }


    public static void InvokeWorldCreated()
    {
        OnWorldCreated?.Invoke();
    }

    public static void InvokeWorldReset()
    {
        OnWorldReset?.Invoke();
    }

    public static void InvokeWorldCenterReady(Vector3 centerPosition)
    {
        OnWorldCenterReady?.Invoke(centerPosition);
    }


    public static void InvokeChunkCreated(Chunk chunk, Vector2 worldCenter)
    {
        OnChunkCreated?.Invoke(chunk, worldCenter);
    }

    // ��� �̺�Ʈ ���� ���� -- ���� ����ǰ� �� ��ȯ�ɶ� ȣ��
    public static void ClearAllEvents()
    {
        OnBlockDamaged = null;
        OnBlockBroken = null;
        OnWorldCreated = null;
        OnWorldReset = null;
        OnWorldCenterReady = null;
        OnChunkCreated = null;
    }
}
