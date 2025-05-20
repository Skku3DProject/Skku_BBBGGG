using UnityEngine;

public abstract class TowerBase : MonoBehaviour, DamageAble
{

    [Header("����Ÿ")]
    [SerializeField] protected TowerData _data;
    public TowerData Data => _data;


    [Header("����")]
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

        // Ÿ�� ���� ���
        Vector3 dir = (targetEnemy.position - _topTowerTransform.position).normalized;

        // ���� ȸ���� ���� (Y�ุ)
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
