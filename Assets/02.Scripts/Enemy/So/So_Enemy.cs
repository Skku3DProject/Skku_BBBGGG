using UnityEngine;

public enum EEnemyType
{
    Normal,
    Boss,
    Elite,


    Count,
}
public enum EEnemyMoveType
{
    Ground,
    Fly,
    GroundFly,


    Count,
}

[CreateAssetMenu(fileName = "EnemySo", menuName = "Scriptable Objects/EnemySo")]
public class So_Enemy : ScriptableObject
{
    [Header("Key")]
    public string Key;
    public string ProjectileKey;
    public string BreathKey;
    public string AreaObjectKey;

    [Header("����")]
    public EEnemyState[] AvailableStates;
    public EEnemyType EnemyType;
    public EEnemyMoveType EnemyMoveType;
    public EEnemyAttackType[] EEnemyAttackTypes;

    [Header("�ð�")]
    public float DamagedTime = 0.5f;

    [Header("����")]
    public int MaxAttackPatternCount = 1;
    public float KnockbackPower = 100;
    public float DamageValue = 10;
    public int MaxAreaAttackCount = 20;

    [Header("�⺻ ����")]
    public float Health = 100;

    [Header("�� �Ÿ�")]
    public float FindDistance = 7;
    public float AttackDistance = 3;
    public float MovePointDistance = 0.1f;
    public float SeparationDistance = 1;

    [Header("���� ����")]
    public float AreaRange;

    [Header("�̵�")]
    public float IdleTime = 2;
    public float Speed = 3;
    public float Gravity = -9.8f; // �߷�
   
    [Header("���̾�")]
    public LayerMask SearchLayerMask;
    public LayerMask AttackLayerMask;

    [Header("�÷�ŷ �˰���")]
    public float SeparationWeight = 1f;
    public float GoalWeight = 1f;
    public float CohesionWeight = 1f;
    public float AlignmentWeight = 1f;
    public float FlockNeighborRadius = 3f;
    /*
      SeparationWeight = 0 �� �и� ����
      SeparationWeight = 1 �� �и��� ��ǥ ���� ���� ����
      SeparationWeight > 1 �� �и� �켱
     */
}
