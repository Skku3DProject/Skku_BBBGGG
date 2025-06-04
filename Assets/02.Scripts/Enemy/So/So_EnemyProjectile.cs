using UnityEngine;
public enum EnemyProjectileType
{
    Point,
    Area
}

[CreateAssetMenu(fileName = "So_EnemyProjectile", menuName = "Scriptable Objects/So_EnemyProjectile")]
public class So_EnemyProjectile : ScriptableObject
{
    [Header("Key")]
    public string Key;
    public string HitVfxKey;

    [Header("½ºÅÝ")]
    public float FlightTime = 1.5f;
    public float Damage = 10;
    public float KnockbackPower = 0;
    public float LostTime = 10;

    public float AreaRange = 5;

    public EnemyProjectileType Type;
    public LayerMask GroundMask;
    //public LayerMask HitMask;
}
