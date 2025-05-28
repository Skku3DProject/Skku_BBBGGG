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

        return EEnemyState.Move;
    }

    public void End()
    {
        EnemyManager.Instance.ClearGrouping(_enemy);
        _enemy.Animator.SetBool("IsRun", false);
    }

}
