using UnityEngine;

public class EnemyAttackCheckEvent : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    private EnemyProjectile _projectile;
    private Enemy _enemy;
    private static Collider[] _hits = new Collider[8];

    private Damage _damage;

  

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
       
    }
    private void Start()
    {
        _damage = new Damage(_enemy.EnemyData.DamageValue, _enemy.gameObject, _enemy.EnemyData.KnockbackPower);

        if (_enemy.EnemyData.EnemyAttackType != EEnemyAttackType.Ranged)
            return;

       

        if (_projectile == null)
        {
            Debug.Log("EnemyProjectile 클래스 없어요");
        }
    }
    public void RangedAttackSpawn()
    {
        GameObject Projectile = Instantiate(ProjectilePrefab, transform);
        _projectile = Projectile.GetComponent<EnemyProjectile>();
    }

    public void RangedAttackEvent()
    {
        _projectile.Launch(_enemy.Target.transform, transform.position);
    }
    public void MeleeAttackEvent()
    {
        int cnt = Physics.OverlapSphereNonAlloc(
            _enemy.transform.position, _enemy.EnemyData.AttackDistance,
            _hits,
            _enemy.EnemyData.AttackLayerMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < cnt; i++)
        {
            var col = _hits[i];

            if (col.TryGetComponent<IDamageAble>(out var dmg))
            {
                dmg.TakeDamage(_damage);
            }

        }
    }
    public void AreaAttackEvent()
    {
        Collider[] hits = new Collider[8];
        int cnt = Physics.OverlapSphereNonAlloc(
            transform.position, _enemy.EnemyData.AreaRange,
            hits,
             _enemy.EnemyData.AttackLayerMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < cnt; i++)
        {
            if (hits[i].TryGetComponent<IDamageAble>(out var dmg))
            {
                dmg.TakeDamage(_damage);
            }
        }
    }


}
