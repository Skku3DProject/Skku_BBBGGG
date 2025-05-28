using UnityEngine;

public interface IEnemyPoolable 
{
    void OnSpawn();      // Ȱ��ȭ �� ȣ��
    void OnDespawn();    // ��Ȱ��ȭ �� ȣ��
    void ResetState(); // ���� ���� �ʱ�ȭ��
}
