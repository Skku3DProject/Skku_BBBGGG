using UnityEngine;

public enum EEnemyType
{
    Normal,
    Boss,
    Elite,


    Count,
}

[CreateAssetMenu(fileName = "EnemySo", menuName = "Scriptable Objects/EnemySo")]
public class EnemySo : ScriptableObject
{

    [Header("����")]
    public EEnemyState[] AvailableStates;
    public EEnemyType EnemyType;

    [Header("�⺻ ����")]
    public float Health = 100;
    public float Speed = 5;

    [Header("�� �Ÿ�")]
    public float FindDistance = 7;
    public float AttackDistance = 3;
    public float MovePointDistance = 0.1f;
}
