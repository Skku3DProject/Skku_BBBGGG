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

    [Header("����")]
    public EEnemyState[] AvailableStates;
    public EEnemyType EnemyType;
    public EEnemyAttackType EnemyAttackType;

    [Header("���� ��ü")]
    public GameObject ProjectilePrefab;

    [Header("�⺻ ����")]
    public float Health = 100;
    public float KnockbackPower = 100;
    public float DamageValue = 10;

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

    [Header("�и� ����")]
    public float SeparationWeight = 1f;
    public float GoalWeight = 1f;
    public float CohesionWeight = 1f;
    public float AlignmentWeight = 1f;
    /*
      SeparationWeight = 0 �� �и� ����
      SeparationWeight = 1 �� �и��� ��ǥ ���� ���� ����
      SeparationWeight > 1 �� �и� �켱
     */
}
