using UnityEngine;
using System.Collections.Generic;

public enum EEnemyAttackType
{
    MeleeAttack,
    RangedAttack,
    AreaAttack,
    SpawnAttack,

    None,
    Count,
}


public class EnemyAttackState : IFSM
{
    private Enemy _enemy;
    private bool _isAttackPattern = false;
    private int _randomAttackCount = 0;
    private int _maxAttackCount = 0;

    private List<IEnemyAttackTypeStrategy> _behaviorStrategys;

    public EnemyAttackState(Enemy enemy)
    {
        _enemy = enemy;

        if(_enemy.EnemyData.EnemyType != EEnemyType.Normal)
        {
            _isAttackPattern = true;
            _maxAttackCount = _enemy.EnemyData.MaxAttackPatternCount;
        }

        Initialize();
    }

    private void Initialize()
    {
        if (_enemy.EnemyData.EEnemyAttackTypes == null) return;

        _behaviorStrategys = new List<IEnemyAttackTypeStrategy>(_enemy.EnemyData.EEnemyAttackTypes.Length);

        foreach (EEnemyAttackType eEnemyAttackType in _enemy.EnemyData.EEnemyAttackTypes)
        {
            switch (eEnemyAttackType)
            {
                case EEnemyAttackType.MeleeAttack:
                    break;
                case EEnemyAttackType.RangedAttack:
                    break;
                case EEnemyAttackType.AreaAttack:
                    break;
                case EEnemyAttackType.SpawnAttack:
                    break;
                case EEnemyAttackType.None:
                    break;
                case EEnemyAttackType.Count:
                    break;
            }
        }
    }

    public void Start()
    {
        _enemy.Animator.SetBool("IsAttack",true);
        EnemyManager.Instance.ClearGrouping(_enemy);

        if(_isAttackPattern)
        {
            _randomAttackCount = RandomAttackCount();
            Debug.Log("_randomAttackCount : " + _randomAttackCount);
            _enemy.Animator.SetFloat("AttackNum", _randomAttackCount);
        }
    }

    public EEnemyState Update()
    {
        if (_enemy.EnemyData.EnemyMoveType != EEnemyMoveType.Fly)
        {
            Gravity();
        }
        return EEnemyState.Attack;
    }

    public void End()
    {
        _enemy.Animator.SetBool("IsAttack", false);
    }
	private void Gravity()
	{
		if (!_enemy.CharacterController.isGrounded)
		{
			_enemy.GravityVelocity.y += _enemy.EnemyData.Gravity * Time.deltaTime;
		}
		else if (_enemy.GravityVelocity.y < 0)
		{
			_enemy.GravityVelocity.y = -2f;
		}

		_enemy.CharacterController.Move(_enemy.GravityVelocity * Time.deltaTime);         // 중력 이동 반영
	}

    private int RandomAttackCount()
    {
        return Random.Range(0, _maxAttackCount);
    }
}
