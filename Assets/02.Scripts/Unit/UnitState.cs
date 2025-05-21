using UnityEngine;

public class UnitState
{
    protected UnitStateMachine _stateMachine;
    protected CharacterController _characterController;
    protected Unit _unit;

    protected bool _triggerCalled; //나중에 애니메이션 끝났다는거 알려주는 용도로 쓸거임
    private string _animBoolName; // 애니메이션 상태변환 할때 쓸거
    protected float _stateTimer;// 각상태마다 사용할 타이머임

    public virtual void AnimFinishTrigger() => _triggerCalled = true;

    public UnitState(UnitStateMachine stateMachine, Unit unit, string animBoolName)
    {
        _stateMachine = stateMachine;
        _unit = unit;
        _animBoolName = animBoolName;

    }

    //나중에 애니메이션 생기면 string이랑 맞춰서 쓰셈
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
