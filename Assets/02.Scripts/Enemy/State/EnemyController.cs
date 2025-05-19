using System.Collections.Generic;
using UnityEngine;

public enum EEnemyState
{
    Idle,
    Move,
    Attack,
    Damaged,
    Die,
}

public class EnemyController : MonoBehaviour, ITickable
{
    private EEnemyState _currentState = EEnemyState.Idle;
    public EEnemyState CurrentState => _currentState;

    private Dictionary<EEnemyState, IFSM> _stateMap;
    public Dictionary<EEnemyState, IFSM> StateMap;
    

    private Enemy _enemy;

    public void Awake()
    {
        _enemy = GetComponent<Enemy>();
        Initialize();
    }


    private void Initialize()
    {
        _stateMap = new Dictionary<EEnemyState, IFSM>();
        // 딕셔너리에 상태 객체 등록
        foreach (EEnemyState state in _enemy.EnemyData.AvailableStates)
        {
            _stateMap[state] = CreateStateInstance(state);
        }

        if (!_stateMap.ContainsKey(EEnemyState.Idle))
        {
            _stateMap.Add(EEnemyState.Idle, new EnemyIdleState(_enemy));
        }

        if (!_stateMap.ContainsKey(EEnemyState.Move))
        {
            _stateMap.Add(EEnemyState.Move, new EnemyMoveState(_enemy));
        }

        if (!_stateMap.ContainsKey(EEnemyState.Damaged))
        {
            _stateMap.Add(EEnemyState.Damaged, new EnemyDamageState(_enemy));
        }

        if (!_stateMap.ContainsKey(EEnemyState.Die))
        {
            _stateMap.Add(EEnemyState.Die, new EnemyDieState(_enemy));
        }

        _currentState = EEnemyState.Idle;
        _stateMap[_currentState].Start();

    }
    public void Tick()
    {
        EEnemyState nextState = _stateMap[_currentState].Update();
        if (nextState != _currentState)
        {
            ChangeState(nextState);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnDisable();
        }
    }


    private void ChangeState(EEnemyState nextState)
    {
        // 현재 상태 종료
        _stateMap[_currentState].End();
        // 새 상태 진입
        _currentState = nextState;
        _stateMap[_currentState].Start();
    }

    

    private void OnEnable()
    {
        TickManager.Register(this);
    }

    private void OnDisable()
    {
        TickManager.Unregister(this);
    }


    private IFSM CreateStateInstance(EEnemyState state)
    {
        switch (state)
        {
            case EEnemyState.Idle:
            {
                return new EnemyIdleState(_enemy);
            }
            case EEnemyState.Move:
            {
                return new EnemyMoveState(_enemy);
            }
            case EEnemyState.Attack:
            {
                return new EnemyAttackState(_enemy);
            }
            case EEnemyState.Damaged:
            {
                return new EnemyDamageState(_enemy);
            }
            case EEnemyState.Die:
            {
                return new EnemyDieState(_enemy);
            }
        }
        return null;
    }
}
