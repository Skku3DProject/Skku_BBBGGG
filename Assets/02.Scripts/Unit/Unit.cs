using System.Collections.Generic;
using UnityEngine;
public enum EUnitState
{
    Idle,
    Move,
    Attack,
    Return
}
public class Unit : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;
    public Animator Animator => _animator;
    private UnitStateMachine _stateMachine;
    private Dictionary<EUnitState, UnitState> _statesMap;

    private void Awake()
    {
        _stateMachine = new UnitStateMachine();
    }
    void Start()
    {
        //_statesMap
        _stateMachine.InitStateMachine(_statesMap[EUnitState.Idle], this, _statesMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
