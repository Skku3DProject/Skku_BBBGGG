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
}
