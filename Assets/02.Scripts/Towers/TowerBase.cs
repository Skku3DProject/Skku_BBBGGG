using System.Collections.Generic;
using UnityEngine;

public abstract class TowerBase : MonoBehaviour, IDamageAble
{

    [Header("데이타")]
    [SerializeField] protected SO_TowerData _data;
    public SO_TowerData Data => _data;


    [Header("참조")]
    [SerializeField] protected Transform _topTowerTransform;
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected SphereCollider _sphereCollider;
    [SerializeField] protected TowerAttackRange _attackRange;
    [SerializeField] protected GameObject _destroyVfxPrefab;
    [SerializeField] protected Transform _destroyVfxPos;
    
    private FractureExplosion fracture;
    protected float _currentHealth;
    protected Vector3 _faceToEnemyDir;
    protected float _attackTimer;

    protected virtual void Awake()
    {
        _attackRange = GetComponentInChildren<TowerAttackRange>();
        fracture = GetComponentInChildren<FractureExplosion>();
    }
    protected virtual void OnEnable()
    {
        _currentHealth = _data.Health;
        _attackTimer = _data.AttackRate;
    }

    protected virtual void Start()
    {
        _sphereCollider.radius = _data.MaxRange;
    }
    protected virtual void Update()
    {
        TraceNearEnemy();

        _attackTimer -= Time.deltaTime;

        if (_attackTimer < 0)
        {
            if (_attackRange.CanAttakc == false) return;
            Attack();
            _attackTimer = _data.AttackRate;
        }
    }

    private void TraceNearEnemy()
    {

        if (_attackRange._targets.Count == 0) return;

        Transform targetEnemy = _attackRange.NearEnemy.transform;

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

        if (_currentHealth <= 0)
        {
            Instantiate(_destroyVfxPrefab).transform.position = _destroyVfxPos.position;
            // 폭발 실행
            fracture.Explode();
            Invoke(nameof(DisableFragments), 6f);
            gameObject.SetActive(false);


            ObjectPool.Instance.ReturnToPool(gameObject);
        }
    }

    private void DisableFragments()
    {
        fracture.ReturnFragmentsToPool();
    }
}
