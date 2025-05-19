using UnityEngine;

public class EnemyIdleState : IFSM
{

    private Enemy _enemy;

    private float _timer = 0;
    private float StartTimer = 5;
    public EnemyIdleState(Enemy enemy)
    {
        _enemy = enemy;
    }
    public void Start()
    {
        _timer = 0;
    }

    public EEnemyState Update() // 서로 거리 조절
    {
        if(_enemy.TryFindTarget()) // 상대 발견
        {
            return EEnemyState.Move;
        }

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
}
