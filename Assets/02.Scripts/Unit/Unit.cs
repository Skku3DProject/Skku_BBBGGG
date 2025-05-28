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
    public LayerMask EnemyLayer;
    public Transform NearestEnemy;
    public float MoveSpeed = 2f;
    public float AttackRange = 5f;
    public float DetectRange = 10f;

    private void Awake()
    {
        _stateMachine = new UnitStateMachine();
        _animator = GetComponent<Animator>();
        _statesMap = new Dictionary<EUnitState, UnitState>()
        {
            {EUnitState.Idle , new UnitIdleState(_stateMachine, this, "Idle") },
            {EUnitState.Move, new UnitMoveState(_stateMachine,this,"Move") },
            {EUnitState.Attack, new UnitAttackState(_stateMachine,this,"Attack") }
        };
    }
    private void OnEnable()
    {
        //_statesMap
        _stateMachine.InitStateMachine(_statesMap[EUnitState.Idle], this, _statesMap);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
    }
}
