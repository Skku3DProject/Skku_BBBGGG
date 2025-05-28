using UnityEngine;

public interface IEnemyPoolable 
{
    void OnSpawn();      // 활성화 시 호출
    void OnDespawn();    // 비활성화 시 호출
    void ResetState(); // 상태 완전 초기화용
}
