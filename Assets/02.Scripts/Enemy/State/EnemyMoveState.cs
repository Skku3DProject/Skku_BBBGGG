using UnityEngine;

public class EnemyMoveState : IFSM
{

    private Enemy _enemy;
    public EnemyMoveState (Enemy enemy)
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
