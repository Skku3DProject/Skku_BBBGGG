using UnityEngine;

public class EnemyDamageState : IFSM
{
    private Enemy _enemy;
    public EnemyDamageState(Enemy enemy)
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
