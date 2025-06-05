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
        if (_enemy.TryAttack()) // ���� ���� �Ұ���
        {

            return EEnemyState.Attack;
        }

        _behaviorStrategy.Move(_enemy, _enemy.EnemyData.Speed);

        // ���� �ֵ� ��� �Ұ�����.
        // 1, ���� ������ ��ġ
        // 2. ���� �ؼ� ���ư���
        // 3. �Ӹ� �� �� �νñ�

        if (_enemy.EnemyData.EnemyMoveType == EEnemyMoveType.Fly)
        {
            return EEnemyState.Move;
        }

        if (Physics.Raycast(_enemy.transform.position, Vector3.down, out RaycastHit hit, 0.5f, _enemy.EnemyData.GroundLayerMask))
        {
            _enemy.SetStepOffset();
        }
        else
        {
            _enemy.CrearStepOffset();
        }

        return EEnemyState.Move;
    }

    public void End()
    {
        _enemy.CrearStepOffset();
        EnemyManager.Instance.ClearGrouping(_enemy);
        _enemy.Animator.SetBool("IsRun", false);
    }

}
