using UnityEngine;

public abstract class TowerBase : MonoBehaviour, DamageAble
{

    [Header("데이타")]
    [SerializeField] protected TowerData _data;
    public TowerData Data => _data;


    [Header("참조")]
    [SerializeField] protected Transform _topTowerTransform;
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected SphereCollider sphereCollider;
    [SerializeField] protected TowerAttackRange _attackRange;

    protected float _currentHealth;
    protected Vector3 _faceToEnemyDir;


    protected float _attackTimer;

    protected virtual void Awake()
    {
        _attackRange = GetComponentInChildren<TowerAttackRange>();
    }
    protected virtual void OnEnable()
    {
        _currentHealth = _data.Health;

        _attackTimer = _data.AttackRate;
    }

    protected virtual void Start()
    {
        sphereCollider.radius = _data.Range;
    }
    protected virtual void Update()
    {
        TraceNearEnemy();

        _attackTimer -= Time.deltaTime;

        if(_attackTimer<0)
        {
            if (_attackRange.CanAttakc == false) return;
            Attack();
            _attackTimer = _data.AttackRate;
        }
    }

    private void TraceNearEnemy()
    {
        Transform targetEnemy = _attackRange.NearEnemy;

        if (targetEnemy == null)
            return;

        // 타겟 방향 계산
        Vector3 dir = (targetEnemy.position - _topTowerTransform.position).normalized;

        // 수평 회전만 적용 (Y축만)
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            _topTowerTransform.rotation = Quaternion.LookRotation(dir);
        }
    }

    protected virtual void Attack()
    {
    }

    public void TakeDamage(Damage damage)
    {
        _currentHealth -= damage.Value;
    }


}
