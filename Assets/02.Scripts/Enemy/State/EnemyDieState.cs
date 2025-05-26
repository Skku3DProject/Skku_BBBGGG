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
        _enemy.Animator.SetTrigger("Die");
        
        EnemyManager.Instance.UnEnable(_enemy);
        UI_Enemy.Instance.TurnOffHpBar(_enemy);
       
    }

    public EEnemyState Update()
    {
        return EEnemyState.Die;
    }

    public void End()
    {

    }
}
