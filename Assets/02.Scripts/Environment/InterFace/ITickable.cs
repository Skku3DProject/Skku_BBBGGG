
using UnityEngine;

public interface ITickable
{
    void Tick();
    int LodLevel { get; }      // 0 = Near, 1 = Mid, 2 = Far
    float LodDistance { get; }   // 플레이어로부터 이 거리까지 이 레벨
    Vector3 position { get; }
 
}
