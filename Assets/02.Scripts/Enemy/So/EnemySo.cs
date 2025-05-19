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

    [Header("상태")]
    public EEnemyState[] AvailableStates;
    public EEnemyType EnemyType;

    [Header("기본 스텟")]
    public float Health = 100;
    public float Speed = 5;
}
