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
        Initialize();// Ȱ��ȭ �� ȣ��

        UI_EnemyHpbar ui_EnemyHpbar = UI_Enemy.Instance.SetHpBarToEnemy(_enemy);
        ui_EnemyHpbar.gameObject.SetActive(false);
        _enemy.SetUi(ui_EnemyHpbar);
        _enemy.Initialize();
        EnemyManager.Instance.OnActivity(_enemy);
    }
    public void OnDespawn()
    {
        // ��Ȱ��ȭ �� ȣ��
        UI_Enemy.Instance.TurnOffHpBar(_enemy);
        EnemyManager.Instance.UnActivity(_enemy);
    }
    public void ResetState()
    {
        // ���� ���� �ʱ�ȭ��
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
        // ��ųʸ��� ���� ��ü ���
        foreach (EEnemyState state in _enemy.EnemyData.AvailableStates)
        {
            _stateMap[state] = CreateStateInstance(state);
        }

        // �⺻���� ������ �־���� ������Ʈ ���
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
        // ���� ���� ����
        _stateMap[_currentState].End();
        // �� ���� ����
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
        // ���� ������ 
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
        targetPosition.y = _enemy.transform.position.y; // Y�� ����
        _enemy.transform.LookAt(targetPosition);
        _enemyVisual.PlayHitFeedback(_enemy.EnemyData.DamagedTime);

        ApplyKnockback(damage);

    }
    public void ApplyKnockback(Damage damage)
    {
        damage.Direction += Vector3.up * 1f; // ��¦ �밢�� ����
        damage.Direction.Normalize();
        // ���� �˹� ���̸� �ߺ� ����

        StartCoroutine(KnockbackCoroutine(damage));
    }

    private IEnumerator KnockbackCoroutine(Damage damage)
    {
        float elapsed = 0f;

        while (elapsed <= 0.1)
        {
            // CharacterController �Ǵ� ���� ������Ʈ�� ��Ȱ��ȭ ���¸� ����
            if (_enemy.CharacterController == null ||
                !_enemy.CharacterController.enabled ||
                !_enemy.CharacterController.gameObject.activeInHierarchy)
            {
                yield break;
            }
            // ������ ������ ���� �Ÿ� �̵�
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

    // �׾����� �ؾ��ϴ� �ൿ
    public void EndDieAnimEvent()
    {
        EnemyPoolManager.Instance.ReturnObject(_enemy.EnemyData.Key, _enemy.gameObject);
    }
}
