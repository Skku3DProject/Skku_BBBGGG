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
        _enemy.Animator.SetTrigger("Attack");
    }

    public EEnemyState Update()
    {
        return EEnemyState.Attack;
    }

    public void End()
    {

    }
}
