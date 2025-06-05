using System.Collections;
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

public class EnemyController : MonoBehaviour, IDamageAble, IEnemyPoolable //, ITickable
{
    private EEnemyState _currentState = EEnemyState.Idle;
    public EEnemyState CurrentState => _currentState;

    private Dictionary<EEnemyState, IFSM> _stateMap;
    public Dictionary<EEnemyState, IFSM> StateMap;

    private Enemy _enemy;

    private EnemyVisual _enemyVisual;


    public bool IsAttack = false;

    public void OnSpawn()
    {
        Initialize();// 활성화 시 호출

        UI_EnemyHpbar ui_EnemyHpbar = UI_Enemy.Instance.SetHpBarToEnemy(_enemy);
        ui_EnemyHpbar.gameObject.SetActive(false);
        _enemy.SetUi(ui_EnemyHpbar);
        _enemy.Initialize();
        EnemyManager.Instance.OnActivity(_enemy);
    }
    public void OnDespawn()
    {
        // 비활성화 시 호출
        UI_Enemy.Instance.TurnOffHpBar(_enemy);
        EnemyManager.Instance.UnActivity(_enemy);
    }
    public void ResetState()
    {
        // 상태 완전 초기화용
    }
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _enemyVisual = GetComponent<EnemyVisual>();
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

        // 기본으로 가지고 있어야할 스테이트 목록
        if (!_stateMap.ContainsKey(EEnemyState.Idle))
        {
            _stateMap.Add(EEnemyState.Idle, new EnemyIdleState(_enemy));
        }

        if (!_stateMap.ContainsKey(EEnemyState.Move))
        {
            _stateMap.Add(EEnemyState.Move, new EnemyMoveState(_enemy));
        }

        if (!_stateMap.ContainsKey(EEnemyState.Attack))
        {
            _stateMap.Add(EEnemyState.Attack, new EnemyAttackState(_enemy));
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
        EnemyManager.Instance.ClearGrouping(_enemy);

        if (_currentState == EEnemyState.Damaged || _currentState == EEnemyState.Die)
        {
            return;
        }
        // 실제 데미지 
        _enemy.TakeDamage(damage);
        OnHit(damage);
        if (_enemy.Health <= 0)
        {
            if (_enemy.EnemyData.EnemyType == EEnemyType.Self_Destruct)
            {
                ChangeState(EEnemyState.Attack);
                return;
            }

            ChangeState(EEnemyState.Die);
            return;
        }

        if (IsAttack)
        {
            return;
        }


        ChangeState(EEnemyState.Damaged);
    }
    private void OnHit(Damage damage)
    {
        Vector3 targetPosition = _enemy.Target.transform.position;
        targetPosition.y = _enemy.transform.position.y; // Y축 고정
        _enemy.transform.LookAt(targetPosition);
        _enemyVisual.PlayHitFeedback(_enemy.EnemyData.DamagedTime);

        ApplyKnockback(damage);

    }
    public void ApplyKnockback(Damage damage)
    {
        damage.Direction += Vector3.up * 1f; // 살짝 대각선 위로
        damage.Direction.Normalize();
        // 기존 넉백 중이면 중복 방지

        StartCoroutine(KnockbackCoroutine(damage));
    }

    private IEnumerator KnockbackCoroutine(Damage damage)
    {
        float elapsed = 0f;

        while (elapsed <= 0.1)
        {
            // CharacterController 또는 게임 오브젝트가 비활성화 상태면 종료
            if (_enemy.CharacterController == null ||
                !_enemy.CharacterController.enabled ||
                !_enemy.CharacterController.gameObject.activeInHierarchy)
            {
                yield break;
            }
            // 프레임 단위로 일정 거리 이동
            _enemy.CharacterController.Move(damage.Direction * damage.KnockbackPower * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    public void EndAttackAnimEvent()
    {
        ChangeState(EEnemyState.Move);
        IsAttack = false;
    }

    // 죽었을때 해야하는 행동
    public void EndDieAnimEvent()
    {
        EnemyPoolManager.Instance.ReturnObject(_enemy.EnemyData.Key, _enemy.gameObject);
    }
}
