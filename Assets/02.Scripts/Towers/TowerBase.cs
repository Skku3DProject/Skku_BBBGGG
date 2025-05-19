using UnityEngine;

public abstract class TowerBase : MonoBehaviour, DamageAble
{

    [Header("데이타")]
    [SerializeField] protected TowerData _data;
    public TowerData Data => _data;


    [Header("참조")]
    [SerializeField] private Transform _topTowerTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private SphereCollider sphereCollider;


    protected float _currentHealth;
    protected Vector3 _faceToEnemyDir;

    
    protected virtual void OnEnable()
    {
        _currentHealth = _data.Health;
    }

    protected virtual void Start()
    {
        sphereCollider.radius = _data.Range;
    }

    protected virtual void Attack()
    {
        TraceEnemy();
    }

    private void TraceEnemy()
    {
        // 어택 할때 적 추적로직
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
    }
}
