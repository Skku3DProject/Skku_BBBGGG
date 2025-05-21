using Unity.VisualScripting;
using UnityEngine;

public class AttackCheckEvent : MonoBehaviour
{

    public GameObject Prefab;
    private Enemy _enemy;
    private static Collider[] _hits = new Collider[8];

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    public void MeleeAttackEvent()
    {
        var pos = _enemy.transform.position;
        float range = _enemy.EnemyData.AttackDistance;

        int cnt = Physics.OverlapSphereNonAlloc(
            pos, range, _hits,
            LayerMask.GetMask("Player", "Camp"),
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < cnt; i++)
        {
            var col = _hits[i];
           /* 
            if (col.TryGetComponent<IDamageable>(out var dmg))
            {
                var damage = new Damage(
                    _enemy.EnemyData.Power,
                    _enemy.gameObject,
                    _enemy.EnemyData.KnockbackPower
                );
                dmg.TakeDamage(damage);
            }
            */
        }
    }
    /*
    void DoBreathAttack()
    {
        Collider[] hits = new Collider[8];
        int cnt = Physics.OverlapSphereNonAlloc(
            transform.position, breathRange, hits,
            LayerMask.GetMask("Player", "Camp"),
            QueryTriggerInteraction.Ignore
        );
        for (int i = 0; i < cnt; i++)
            if (hits[i].TryGetComponent<IDamageable>(out var dmg))
                dmg.TakeDamage(breathDamage);
    }

    // 2) 느린 투사체 발사
    void FireProjectile()
    {
        var proj = ProjectilePool.Get();
        proj.Launch(transform.position, targetDir, speed, damage);
    }

    // 3) 즉시 명중 원거리 (Hitscan)
    void FireHitscan()
    {
        if (Physics.Raycast(barrelPos, aimDir, out var hit, maxRange, playerMask))
            if (hit.collider.TryGetComponent<IDamageable>(out var dmg))
                dmg.TakeDamage(hitDamage);
    }

    // 원거리
    public void RangedAttackEvent()
    {
        Instantiate(Prefab);
    }
    */
    public void EndAttackEvent()
    {
    }
    
}
