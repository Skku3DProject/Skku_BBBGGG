using UnityEngine;

public class EnemyDieState : IFSM
{
    private Enemy _enemy;
    public EnemyDieState(Enemy enemy)
    {
        _enemy = enemy;
    }
    public void Start()
    {

    }

    public EEnemyState Update()
    {
        return EEnemyState.Idle;
    }

    public void End()
    {

    }
}
