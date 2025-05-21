using UnityEngine;

public class UnitState
{
    protected UnitStateMachine _stateMachine;
    protected CharacterController _characterController;
    protected Unit _unit;

    protected bool _triggerCalled; //���߿� �ִϸ��̼� �����ٴ°� �˷��ִ� �뵵�� ������
    private string _animBoolName; // �ִϸ��̼� ���º�ȯ �Ҷ� ����
    protected float _stateTimer;// �����¸��� ����� Ÿ�̸���

    public virtual void AnimFinishTrigger() => _triggerCalled = true;

    public UnitState(UnitStateMachine stateMachine, Unit unit, string animBoolName)
    {
        _stateMachine = stateMachine;
        _unit = unit;
        _animBoolName = animBoolName;

    }

    //���߿� �ִϸ��̼� ����� string�̶� ���缭 ����
    protected virtual void SetAnimation(bool value)
    {
        //if (!string.IsNullOrEmpty(_animBoolName))
        //    _unit.Animator.SetBool(_animBoolName, value);
    }

    public virtual void Enter()
    {

        _stateTimer = 0;

        SetAnimation(true);
        _triggerCalled = false;

        if (_unit.gameObject.activeSelf != true) return;

    }
    public virtual void Update()
    {
        _stateTimer -= Time.deltaTime;
        if (_unit.gameObject.activeSelf != true) return;
    }
    public virtual void Exit()
    {
        SetAnimation(false);
        if (_unit.gameObject.activeSelf != true) return;
    }

    public void AnimTrigger() => _triggerCalled = true;
}
