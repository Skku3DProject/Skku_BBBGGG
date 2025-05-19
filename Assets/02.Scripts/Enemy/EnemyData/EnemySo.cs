using UnityEngine;

public enum EEnemyType
{
    Normal,
    Boss,
    Elite,
}

[CreateAssetMenu(fileName = "EnemySo", menuName = "Scriptable Objects/EnemySo")]
public class EnemySo : ScriptableObject
{
    public EEnemyType EnemyType;

    [Header("±âº» ½ºÅÝ")]
    public float Health = 100;
    public float Speed = 5;
}
