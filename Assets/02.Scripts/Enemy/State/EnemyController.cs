using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum EEnemyState
{
    Idle,
    Move,
    Attack,
    Damaged,
    Die,
}

public class EnemyController : MonoBehaviour//, ITickable
{
    private EEnemyState _currentState = EEnemyState.Idle;
    public EEnemyState CurrentState => _currentState;

    private Dictionary<EEnemyState, IFSM> _stateMap;
    public Dictionary<EEnemyState, IFSM> StateMap;

    private Enemy _enemy;
    private Vector3 _gravityVelocity;
    private const float GRAVITY = -9.8f; // �߷�

    public void Awake()
    {
        _enemy = GetComponent<Enemy>();
        Initialize();
    }


    private void Initialize()
    {
        _stateMap = new Dictionary<EEnemyState, IFSM>();
        // ��ųʸ��� ���� ��ü ���
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


    public void Update()
    {
        Gravity();
        EEnemyState nextState = _stateMap[_currentState].Update();
        if (nextState != _currentState)
        {
            ChangeState(nextState);
        }
    }


    private void ChangeState(EEnemyState nextState)
    {
        // ���� ���� ����
        _stateMap[_currentState].End();
        // �� ���� ����
        _currentState = nextState;
        _stateMap[_currentState].Start();
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

    private void Gravity()
    {
        if (!_enemy.CharacterController.isGrounded)
        {
            _gravityVelocity.y += GRAVITY * Time.deltaTime;
        }
        else if (_gravityVelocity.y < 0)
        {
            _gravityVelocity.y = -2f;
        }
        
        _enemy.CharacterController.Move(_gravityVelocity * Time.deltaTime);         // �߷� �̵� �ݿ�
    }

    /*
 // LOD ����: Near/Mid/Far �Ÿ�
 [SerializeField] float nearDistance = 10f;
 [SerializeField] float midDistance = 20f;
 [SerializeField] float farDistance = 40f;

 private CharacterController _cc;

 public int LodLevel
 {
     get
     {
         float dist = Vector3.Distance(transform.position, _enemy.Target.transform.position);
         if (dist <= nearDistance) return 0;
         if (dist <= midDistance) return 1;
         return 2;
     }
 }

 public float LodDistance
 {
     get
     {
         // �� ������ �ִ� �Ÿ� ���� ��ȯ
         return LodLevel == 0 ? nearDistance
              : LodLevel == 1 ? midDistance
                               : farDistance;
     }
 }

 public Vector3 position => transform.position;

public void Tick()
{
 EEnemyState nextState = _stateMap[_currentState].Update();
 if (nextState != _currentState)
 {
     ChangeState(nextState);
 }
}

private void OnEnable()
{
 TickManager.Register(this);
 Debug.Log("OnEnable");
}

private void OnDisable()
{
 TickManager.Unregister(this);
 Debug.Log("OnDisable");
}
    */
}
