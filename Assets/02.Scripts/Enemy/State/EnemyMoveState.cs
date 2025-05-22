using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMoveState : IFSM
{

    private Enemy _enemy;
    private float _speed;
    private Vector3 Direction = Vector3.zero;
    public EnemyMoveState(Enemy enemy)
    {
        _enemy = enemy;
        Initialize();
    }

    private void Initialize()
    {
        _speed = _enemy.EnemyData.Speed;
    }

    public void Start()
    {
        _enemy.Animator.SetBool("IsRun" , true);
    }

    public EEnemyState Update()
    {
        if (_enemy.TryAttack()) // ���� ���� �Ұ���
        {
            EnemyManager.Instance.Unregister(_enemy);
            return EEnemyState.Attack;
        }

        _enemy.TargetOnPlayer();

        MoveWithSeparationAndGravity();

        return EEnemyState.Move;
    }

    public void End()
    {
        _enemy.Animator.SetBool("IsRun", false);
    }

    private void MoveWithSeparationAndGravity()
    {
        Vector3 pos = _enemy.transform.position;

        // 1) ��ǥ ����
        Vector3 goalDir = (_enemy.Target.transform.position - pos).normalized;

        // 2) �и� ���� ���
        Vector3 sepDir = CalculateSeparation(pos);

        // 3) �̵� ���� �ջ�
        float goalWeight = _enemy.EnemyData.GoalWeight;
        float sepWeight = _enemy.EnemyData.SeparationWeight;
        Vector3 moveDir = (goalDir * goalWeight + sepDir * sepWeight).normalized;

        // 4) �߷� ó��
        if (_enemy.CharacterController.isGrounded && _enemy.GravityVelocity.y < 0)
        {
            _enemy.GravityVelocity.y = -2f;
        }

        _enemy.GravityVelocity.y += _enemy.EnemyData.Gravity * Time.deltaTime;

        // 5) ���� �̵� �� ����
        Vector3 finalMove = moveDir * _speed + _enemy.GravityVelocity;
        _enemy.CharacterController.Move(finalMove * Time.deltaTime);

        Vector3 targetPosition = _enemy.Target.transform.position;
        targetPosition.y = _enemy.transform.position.y; // Y�� ����
   
        _enemy.transform.LookAt(targetPosition);


    }

    private Vector3 CalculateSeparation(Vector3 pos)
    {
        List<Enemy> neighbors = EnemyManager.Instance.GetNeighbors(_enemy);
        Vector3 force = Vector3.zero;
        int count = 0;

        foreach (var other in neighbors)
        {
            if (other == _enemy) continue;
            Vector3 diff = pos - other.transform.position;
            float dist = diff.magnitude;
            if (dist < _enemy.EnemyData.SeparationDistance && dist > 0.001f)
            {
                force += diff.normalized / dist;
                count++;
            }
        }

        if (count > 0)
            force /= count;

        return force.normalized;
    }
}
