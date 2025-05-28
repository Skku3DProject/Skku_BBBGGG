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
        if (hits.Length > 0)
        {
            _unit.NearestEnemy = hits[0].transform; // ���� ����� ���� ã�� ���� ���� �Ұ� �ӽÿ�
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
