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

        float dist = Vector3.Distance(_unit.transform.position, _unit.NearestEnemy.position);
        if (dist <= _unit.AttackRange)
        {
            _stateMachine.ChangeState(EUnitState.Attack);
            return;
        }

        Vector3 dir = (_unit.NearestEnemy.position - _unit.transform.position).normalized;

        Vector3 lookDir = new Vector3(dir.x, 0, dir.z);
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            _unit.transform.rotation = Quaternion.Slerp(_unit.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        _unit.transform.position += dir * _unit.MoveSpeed * Time.deltaTime;
    }

}
