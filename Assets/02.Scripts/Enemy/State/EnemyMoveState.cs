using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : IFSM
{

    private Enemy _enemy;
    private float _speed;
    private Vector3 _prevDirection = Vector3.zero;
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
        _enemy.TargetOnPlayer();
        if (_enemy.TryAttack()) // 공격 가능 불가능
        {
            EnemyManager.Instance.Unregister(_enemy);
            return EEnemyState.Attack;
        }


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

        Vector3 goalDir = (_enemy.Target.transform.position - pos).normalized;
        Vector3 sepDir = CalculateSeparation(pos);
        Vector3 cohDir = CalculateCohesion(pos);
        Vector3 alignDir = CalculateAlignment();

        float goalWeight = _enemy.EnemyData.GoalWeight;
        float sepWeight = _enemy.EnemyData.SeparationWeight;
        float cohWeight = _enemy.EnemyData.CohesionWeight;
        float alignWeight = _enemy.EnemyData.AlignmentWeight;

        Vector3 desiredDir = (goalDir * goalWeight + sepDir * sepWeight + cohDir * cohWeight + alignDir * alignWeight).normalized;

        Vector3 moveDir = Vector3.Lerp(_prevDirection, desiredDir, 0.15f).normalized;
        _prevDirection = moveDir;

        // 4) 중력 처리
        if (_enemy.CharacterController.isGrounded && _enemy.GravityVelocity.y < 0)
        {
            _enemy.GravityVelocity.y = -2f;
        }
        _enemy.GravityVelocity.y += _enemy.EnemyData.Gravity * Time.deltaTime;

        // 5) 최종 이동 및 적용
        Vector3 finalMove = moveDir * _speed + _enemy.GravityVelocity;
        _enemy.CharacterController.Move(finalMove * Time.deltaTime);

        Vector3 targetPosition = _enemy.Target.transform.position;
        targetPosition.y = _enemy.transform.position.y; // Y축 고정
        _enemy.transform.LookAt(targetPosition);
        _enemy.CurrentMoveDirection = _prevDirection;
    }

    private Vector3 CalculateSeparation(Vector3 pos)
    {
        if (!EnemyManager.Instance.TryCheckRegister(_enemy))
        {
            return Vector3.zero;
        }

        List<Enemy> neighbors = EnemyManager.Instance.GetNeighbors(_enemy);

        Vector3 force = Vector3.zero;
        int count = 0;

        foreach (Enemy other in neighbors)
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
        {
            force /= count;
        }

        return force.normalized;
    }

    private Vector3 CalculateCohesion(Vector3 pos)
    {
        if(!EnemyManager.Instance.TryCheckRegister(_enemy))
        {
            return Vector3.zero;
        }

        List<Enemy> neighbors = EnemyManager.Instance.GetNeighbors(_enemy);
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (Enemy other in neighbors)
        {
            if (other == _enemy) continue;
            center += other.transform.position;
            count++;
        }

        if (count > 0)
        {
            center /= count;
            return (center - pos).normalized;
        }

        return Vector3.zero;
    }
    private Vector3 CalculateAlignment()
    {
        if (!EnemyManager.Instance.TryCheckRegister(_enemy))
        {
            return Vector3.zero;
        }

        List<Enemy> neighbors = EnemyManager.Instance.GetNeighbors(_enemy);
        Vector3 averageDir = Vector3.zero;
        int count = 0;

        foreach (Enemy other in neighbors)
        {
            if (other == _enemy) continue;
            averageDir += other.CurrentMoveDirection;
            count++;
        }

        if (count > 0)
        {
            averageDir /= count;
            return averageDir.normalized;
        }

        return Vector3.zero;
    }
}
