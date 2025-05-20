using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class EnemyIdleState : IFSM
{

    private Enemy _enemy;

    private float _timer = 0;
    private float StartTimer = 2;
    public EnemyIdleState(Enemy enemy)
    {
        _enemy = enemy;
    }
    public void Start()
    {
        _timer = 0;
    }

    public EEnemyState Update() // ���� �Ÿ� ����
    {
        Gravity();
       
        _timer += Time.deltaTime;
        if (_timer > StartTimer)
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

        _enemy.CharacterController.Move(_enemy.GravityVelocity * Time.deltaTime);         // �߷� �̵� �ݿ�
    }
}
