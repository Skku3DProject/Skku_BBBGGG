using UnityEngine;

public class UnitIdleState : UnitState
{
    float _detectTime = 0.5f;

    public UnitIdleState(UnitStateMachine stateMachine, Unit enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _stateTimer = _detectTime;


    }

    public override void Exit()
    {
        base.Exit();
    }
    void SearchForEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(_unit.transform.position, _unit.DetectRange, _unit.EnemyLayer);

        GameObject nearestNonFlyEnemy = null;
        float nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            Enemy enemy = hit.gameObject.GetComponent<Enemy>();
            if (enemy == null) continue;

            // 비행 적이면 무시
            if (enemy.EnemyData.EnemyMoveType == EEnemyMoveType.Fly)
                continue;

            float dist = Vector3.Distance(_unit.transform.position, hit.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestNonFlyEnemy = hit.gameObject;
            }
        }

        if (nearestNonFlyEnemy != null)
        {
            _unit.NearestEnemy = nearestNonFlyEnemy;
            _stateMachine.ChangeState(EUnitState.Move);
        }
    }
    public override void Update()
    {
        base.Update();
        Debug.Log("Enter Idle");
        if (_stateTimer<0)
        {
            _stateTimer = _detectTime;

            SearchForEnemy();
        }
    }
}
