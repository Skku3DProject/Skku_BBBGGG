using UnityEngine;

public class EnemyMoveState : IFSM
{
    private Enemy _enemy;
    private IEnemyMoveTypeStrategy _behaviorStrategy;

    public EnemyMoveState(Enemy enemy)
    {
        _enemy = enemy;
        Initialize();
    }

    private void Initialize()
    {
        switch (_enemy.EnemyData.EnemyMoveType)
        {
            case EEnemyMoveType.Fly:
                _behaviorStrategy = new FlyEnemyStrategy();
                break;
            case EEnemyMoveType.Ground:
                _behaviorStrategy = new GroundEnemyStrategy();
                break;
            default:
                _behaviorStrategy = new GroundAndFlyEnemyStrategy();
                break;
        }
    }

    public void Start()
    {
        EnemyManager.Instance.SetMoveTypeGrouping(_enemy);
        _enemy.Animator.SetBool("IsRun", true);
    }

    public EEnemyState Update()
    {
        _enemy.TargetOnPlayer();
        if (_enemy.TryAttack()) // 공격 가능 불가능
        {

            return EEnemyState.Attack;
        }

        _behaviorStrategy.Move(_enemy, _enemy.EnemyData.Speed);

        // 갇힌 애들 어떻게 할것인지.
        // 1, 발판 같은거 설치
        // 2. 점프 해서 날아가기
        // 3. 머리 위 블럭 부시기
		/*RaycastHit hit;

		if (Physics.Raycast(_enemy.UI_offset.position,Vector3.forward))
        {

        }*/
			return EEnemyState.Move;
    }

    public void End()
    {
        EnemyManager.Instance.ClearGrouping(_enemy);
        _enemy.Animator.SetBool("IsRun", false);
    }

}
