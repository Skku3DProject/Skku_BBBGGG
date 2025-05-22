using UnityEngine;

public class EnemyAttackCheckEvent : MonoBehaviour
{
    public GameObject Prefab;
    private Enemy _enemy;
    private static Collider[] _hits = new Collider[8];

    private Damage _damage;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        _damage = new Damage(_enemy.EnemyData.DamageValue, _enemy.gameObject, _enemy.EnemyData.KnockbackPower);
    }

    public void RangedAttackSpawn()
    {
        // 자식에 풀링으로 소환되어있는 오브젝트를 팔에 장착.
    }

    public void RangedAttackEvent()
    {
        // 발사
        Debug.Log("RangedAttackEvent");
    }

    public void MeleeAttackEvent()
    {
        Debug.Log("MeleeAttackEvent");

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
    // 원거리
 

    // 범위 공격
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
