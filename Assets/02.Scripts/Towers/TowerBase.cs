using UnityEngine;

public abstract class TowerBase : MonoBehaviour, DamageAble
{

    [Header("����Ÿ")]
    [SerializeField] protected TowerData _data;
    public TowerData Data => _data;


    [Header("����")]
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
        // ���� �Ҷ� �� ��������
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
    }
}
