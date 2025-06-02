using System.Collections.Generic;
using UnityEngine;

public enum ETowerType
{
    ArrowTower,
    FireTower,
    IceTower
}

public abstract class TowerBase : MonoBehaviour, IDamageAble
{

    [Header("����Ÿ")]
    [SerializeField] protected SO_TowerData _data;
    public SO_TowerData Data => _data;
    public ETowerType TowerType;

    [Header("����")]
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
        ShootSound();
    }

    public void TakeDamage(Damage damage)
    {
        _currentHealth -= damage.Value;

        if (_currentHealth <= 0)
        {
            TowerSoundController.Instance.PlaySoundAt(TowerSoundType.Collapse, gameObject.transform.position);

            Instantiate(_destroyVfxPrefab).transform.position = _destroyVfxPos.position;
            // ���� ����
            fracture.Explode();
            Invoke(nameof(DisableFragments), 6f);
            gameObject.SetActive(false);


            ObjectPool.Instance.ReturnToPool(gameObject);
        }
    }

    private void ShootSound()
    {
        switch(TowerType)
        {
            case ETowerType.ArrowTower:
                TowerSoundController.Instance.PlaySoundAt(TowerSoundType.ArrowShoot, gameObject.transform.position);
                break;
            case ETowerType.FireTower:
                TowerSoundController.Instance.PlaySoundAt(TowerSoundType.FireballShoot, gameObject.transform.position);
                break;
            case ETowerType.IceTower:
                TowerSoundController.Instance.PlaySoundAt(TowerSoundType.IceballShoot, gameObject.transform.position);
                break;
        }
    }


    private void DisableFragments()
    {
        //fracture.ReturnFragmentsToPool();
    }
}
