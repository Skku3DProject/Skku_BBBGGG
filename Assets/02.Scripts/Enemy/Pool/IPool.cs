using UnityEngine;

public interface IPool 
{
    void OnSpawn();      // 활성화 시 호출
    void OnDespawn();    // 비활성화 시 호출
}
