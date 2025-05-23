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
        _enemy.Animator.SetBool("IsDie",true);
        _enemy.gameObject.SetActive(false);
        EnemyManager.Instance.UnEnable(_enemy);
        if (_enemy.UI_EnemyHpbar != null)
        {
            _enemy.UI_EnemyHpbar.OnEnemyDestroyed();
        }
    }

    public EEnemyState Update()
    {
        return EEnemyState.Idle;
    }

    public void End()
    {

    }
}
