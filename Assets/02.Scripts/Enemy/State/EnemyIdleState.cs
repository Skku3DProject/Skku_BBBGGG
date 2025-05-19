using UnityEngine;

public class EnemyIdleState : IFSM
{

    private Enemy _enemy;
    public EnemyIdleState(Enemy enemy)
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
