using UnityEngine;

public class UnitMoveState : UnitState
{
    public UnitMoveState(UnitStateMachine stateMachine, Unit enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        MoveToEnemy();
    }

    void MoveToEnemy()
    {
        if (_unit.NearestEnemy == null)
        {
            _stateMachine.ChangeState(EUnitState.Idle);
            return;
        }

        Vector3 dir = (_unit.NearestEnemy.position - _unit.transform.position).normalized;
        _unit.transform.position += dir * _unit.MoveSpeed * Time.deltaTime;

        //float dist = Vector3.Distance(_unit.transform.position, _unit.NearestEnemy.position);
        //if (dist <= 2f)
        //{
        //    _stateMachine.ChangeState(EUnitState.Attack);
        //}
        //else
        //{
        //    Vector3 dir = (_unit.NearestEnemy.position - _unit.transform.position).normalized;
        //    _unit.transform.position += dir * _unit.MoveSpeed * Time.deltaTime;
        //}
    }

}
