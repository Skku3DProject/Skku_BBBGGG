using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public float Health = 0f;
    public float Range = 0f;
    public float Damage = 0f;
    public float AttackRate = 0f;
    public float SplashRadius = 0f;
}
