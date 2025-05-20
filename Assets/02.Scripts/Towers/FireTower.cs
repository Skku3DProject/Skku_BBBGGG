using UnityEngine;

public class FireTower : TowerBase
{
    protected override void Attack()
    {
        base.Attack();
        Vector3 startPos = _topTowerTransform.position;
        GameObject arrow = ObjectPool.Instance.GetObject(_bulletPrefab,startPos,Quaternion.identity);//Instantiate(_bulletPrefab, startPos, Quaternion.identity);

        arrow.GetComponent<ProjectileBase>().Init(
            startPos,
            _attackRange.NearEnemy.position,
            flightDuration: 1.5f,
            data: _data
        );
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
