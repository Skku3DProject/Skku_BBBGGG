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
        _enemy.CharacterController.enabled = false;
    }

    public EEnemyState Update()
    {
        return EEnemyState.Die;
    }

    public void End()
    {

    }
}
