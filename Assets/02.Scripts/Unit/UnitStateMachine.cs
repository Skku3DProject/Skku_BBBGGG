using System.Collections.Generic;
using UnityEngine;

public class UnitStateMachine
{
    public bool IsInited = false;
    private UnitState _currentState;
    public UnitState CurrentState => _currentState;
    private Unit _enemy;
    private Dictionary<EUnitState, UnitState> _stateMap;

    public void InitStateMachine(UnitState currentState, Unit enemy, Dictionary<EUnitState, UnitState> stateMap)
    {
        Debug.Log("InitMachine");
        _currentState = currentState;
        _stateMap = stateMap;
        _enemy = enemy;
        IsInited = true;
    }
    public void ChangeState(UnitState state)
    {
        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }
    public void ChangeState(EUnitState stateType)
    {
        if (_stateMap.TryGetValue(stateType, out var nextState))
        {
            ChangeState(nextState);
        }
        else
        {
            Debug.LogWarning($"없는 상태인데용? 이거 {stateType} ");
        }
    }

    public void Update()
    {
        _currentState.Update();
    }
}
