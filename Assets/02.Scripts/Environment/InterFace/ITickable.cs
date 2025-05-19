
using UnityEngine;

public interface ITickable
{
    void Tick();
    int LodLevel { get; }      // 0 = Near, 1 = Mid, 2 = Far
    float LodDistance { get; }   // �÷��̾�κ��� �� �Ÿ����� �� ����
    Vector3 position { get; }
 
}
