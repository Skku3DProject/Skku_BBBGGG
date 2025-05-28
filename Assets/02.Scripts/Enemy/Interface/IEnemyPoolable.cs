using UnityEngine;

public interface IEnemyPoolable 
{
    public void OnSpawn();      // 활성화 시 호출
    public void OnDespawn();    // 비활성화 시 호출
}
