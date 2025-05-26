using UnityEngine;

public class EnemyIdleState : IFSM
{
    private Enemy _enemy;

    private float _timer = 0;
    private float _idleTime = 2;
    public EnemyIdleState(Enemy enemy)
    {
        _enemy = enemy;
        _idleTime = _enemy.EnemyData.IdleTime;
    }
    public void Start()
    {
        _timer = 0;
    }

    public EEnemyState Update() // 서로 거리 조절
    {
        Gravity();
       
        _timer += Time.deltaTime;
        if (_timer >= _idleTime)
        {
            return EEnemyState.Move;
        }

        return EEnemyState.Idle;
    }

    public void End()
    {
        _timer = 0;
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
