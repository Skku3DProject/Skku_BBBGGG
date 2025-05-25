using UnityEngine;

public class EnemyDamageState : IFSM
{
    private Enemy _enemy;

    private float _damagedTime = 0.5f;
    private float _damagedTimer = 0f;
    public EnemyDamageState(Enemy enemy)
    {
        _enemy = enemy;
    }
    public void Start()
    {
        _damagedTimer = 0;
        _enemy.Animator.SetTrigger("Damage");
    }

    public EEnemyState Update()
    {
        Gravity();

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
