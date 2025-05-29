using UnityEngine;

public class EnemyDamageState : IFSM
{
    private Enemy _enemy;

    private float _damagedTime = 0.5f;
    private float _damagedTimer = 0f;
    public EnemyDamageState(Enemy enemy)
    {
        _enemy = enemy;
        Initialize();
    }

    private void Initialize()
    {
        _damagedTime = _enemy.EnemyData.DamagedTime;
    }
    public void Start()
    {
        _damagedTimer = 0;
        _enemy.Animator.SetTrigger("Damage");
        _enemy.CharacterController.stepOffset = 0;
        EnemyManager.Instance.ClearGrouping(_enemy);
    }

    public EEnemyState Update()
    {
        if (_enemy.EnemyData.EnemyMoveType != EEnemyMoveType.Fly)
        {
            Gravity();
        }

        _damagedTimer += Time.deltaTime;

        if (_damagedTimer > _damagedTime)
        {
            _damagedTimer = 0;
            return EEnemyState.Move;
        }

        return EEnemyState.Damaged;
    }

    public void End()
    {
        _damagedTimer = 0;
        _enemy.CharacterController.stepOffset = _enemy.StepOffset;
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

}
