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

    }

    public EEnemyState Update()
    {
        return EEnemyState.Idle;
    }

    public void End()
    {

    }
}
