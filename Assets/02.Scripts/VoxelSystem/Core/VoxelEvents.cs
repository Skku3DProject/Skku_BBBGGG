using System;
using UnityEngine;

//이벤트들을 여기에 몰아넣음
public static class VoxelEvents
{
    // 블록 관련 이벤트
    public static event Action<Vector3Int> OnBlockDamaged;
    public static event Action<Vector3Int> OnBlockBroken;

    // 월드 관련 이벤트
    public static event Action OnWorldCreated;
    public static event Action OnWorldReset;
    public static event Action<Vector3> OnWorldCenterReady;
    public static event Action<Vector3> OnCreateEnemySpawenr;

    // 청크 관련 이벤트
    public static event Action<Chunk, Vector2> OnChunkCreated;

    //Invoke 메서드들
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

    // 모든 이벤트 구독 해제 -- 게임 종료되고 씬 전환될때 호출
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
