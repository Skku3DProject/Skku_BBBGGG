using UnityEditor;
using UnityEngine;

public class EnemyMoveState : IFSM
{

    private Enemy _enemy;
    private float _speed;
    private Vector3 Direction = Vector3.zero;
    public EnemyMoveState (Enemy enemy)
    {
        _enemy = enemy;
        Initialize();
    }

    private void Initialize()
    {
        _speed = _enemy.EnemyData.Speed;
    }

    public void Start()
    {
        Debug.Log("Move Start");
    }

    public EEnemyState Update()
    {
      
        if(_enemy.TryAttack()) // 공격 가능 불가능
        {
            return EEnemyState.Attack;
        }

      
        Direction = _enemy.Target.transform.position - _enemy.transform.position;
        _enemy.CharacterController.Move(Direction * _speed * Time.deltaTime);

        return EEnemyState.Move;
    }

    public void End()
    {

    }
}
