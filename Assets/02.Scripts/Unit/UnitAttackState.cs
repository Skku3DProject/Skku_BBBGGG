using UnityEngine;

public class UnitAttackState : UnitState
{
    public UnitAttackState(UnitStateMachine stateMachine, Unit enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
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

        if (_unit.IsAttacking)
            _unit.AttackCollider.gameObject.SetActive(true);
        else
            _unit.AttackCollider.gameObject.SetActive(false);

        if (!_unit.NearestEnemy.activeSelf && _triggerCalled)
        {
            _stateMachine.ChangeState(EUnitState.Idle);
            return;
        }

        float dist = Vector3.Distance(_unit.transform.position, _unit.NearestEnemy.transform.position);
        if (dist > _unit.AttackRange && _triggerCalled)
        {
            _stateMachine.ChangeState(EUnitState.Move);
            return;
        }

        // 목표 방향 바라보기 (회전)
        Vector3 dir = (_unit.NearestEnemy.transform.position - _unit.transform.position).normalized;
        Vector3 lookDir = new Vector3(dir.x, 0, dir.z);
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            _unit.transform.rotation = Quaternion.Slerp(_unit.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }


    }
}
