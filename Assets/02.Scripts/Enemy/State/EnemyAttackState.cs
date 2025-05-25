using UnityEngine;

public class EnemyAttackState : IFSM
{
    private Enemy _enemy;
    public EnemyAttackState(Enemy enemy)
    {
        _enemy = enemy;
    }
    public void Start()
    {
        _enemy.Animator.SetBool("IsAttack",true);
    }

    public EEnemyState Update()
    {
		Gravity();
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
}
