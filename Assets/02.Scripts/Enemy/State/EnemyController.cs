using System.Collections.Generic;
using System.Data;
using UnityEngine;

public enum EEnemyState
{
    Idle,
    Move,
    Attack,
    Damaged,
    Die,
}

public class EnemyController : MonoBehaviour, IDamageAble//, ITickable
{
    private EEnemyState _currentState = EEnemyState.Idle;
    public EEnemyState CurrentState => _currentState;

    private Dictionary<EEnemyState, IFSM> _stateMap;
    public Dictionary<EEnemyState, IFSM> StateMap;

    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        SetState();
        Initialize();
    }

    private void SetState()
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

    public void Initialize()
    {
        _currentState = EEnemyState.Idle;
        _stateMap[_currentState].Start();
    }

    public void Update()
    {
        EEnemyState nextState = _stateMap[_currentState].Update();
        if (nextState != _currentState)
        {
            ChangeState(nextState);
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
    
    public void TakeDamage(Damage damage)
    {
        Debug.Log("damage1");
        if (_currentState == EEnemyState.Damaged || _currentState == EEnemyState.Die)
        {
            return;
        }

        Debug.Log("damage2");

        // 넉백
        Vector3 dir = (damage.From.transform.position - transform.position) * -1;
        dir.Normalize();
        _enemy.CharacterController.Move(dir * damage.KnockbackPower * Time.deltaTime);

        _enemy.TakeDamage(damage);

        if (_enemy.Health <= 0)
        {
            ChangeState(EEnemyState.Die);
            return;
        }
        if(damage.From.CompareTag("Player"))
        {
            _enemy.OnPlayer();
        }

        ChangeState(EEnemyState.Damaged);
    }
    public void EndAttackAnimEvent()
    {
        ChangeState(EEnemyState.Move);
        // 어택 종료
    }

    // 죽었을때 해야하는 행동
    public void EndDieAnimEvent()
    {
        gameObject.SetActive(false);
    }

    /*
 // LOD 설정: Near/Mid/Far 거리
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
         // 각 레벨의 최대 거리 기준 반환
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
