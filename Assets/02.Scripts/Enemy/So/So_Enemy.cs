using UnityEngine;

public enum EEnemyType
{
    Normal,
    Boss,
    Elite,


    Count,
}
public enum EEnemyAttackType
{
    Melee,
    Ranged,
    Fly,


    Count,
}

[CreateAssetMenu(fileName = "EnemySo", menuName = "Scriptable Objects/EnemySo")]
public class So_Enemy : ScriptableObject
{

    [Header("상태")]
    public EEnemyState[] AvailableStates;
    public EEnemyType EnemyType;
    public EEnemyAttackType EnemyAttackType;

    [Header("공격 물체")]
    public GameObject ProjectilePrefab;

    [Header("기본 스텟")]
    public float Health = 100;
    public float KnockbackPower = 100;
    public float DamageValue = 10;

    [Header("비교 거리")]
    public float FindDistance = 7;
    public float AttackDistance = 3;
    public float MovePointDistance = 0.1f;
    public float SeparationDistance = 1;

    [Header("공격 범위")]
    public float AreaRange;

    [Header("이동")]
    public float IdleTime = 2;
    public float Speed = 3;
    public float Gravity = -9.8f; // 중력
   
    [Header("레이어")]
    public LayerMask SearchLayerMask;
    public LayerMask AttackLayerMask;

    [Header("분리 강도")]
    public float SeparationWeight = 1f;
    public float GoalWeight = 1f;
    public float CohesionWeight = 1f;
    public float AlignmentWeight = 1f;
    /*
      SeparationWeight = 0 → 분리 무시
      SeparationWeight = 1 → 분리와 목표 방향 동등 비중
      SeparationWeight > 1 → 분리 우선
     */
}
