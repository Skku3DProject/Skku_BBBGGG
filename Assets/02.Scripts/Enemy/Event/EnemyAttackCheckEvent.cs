using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCheckEvent : MonoBehaviour
{
    public List<GameObject> ProjectilePrefabs;

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
        foreach (GameObject prefab in ProjectilePrefabs)
        {
            if (prefab != null || prefab.gameObject.activeInHierarchy == false)
            {

                _projectile = prefab.GetComponent<EnemyProjectile>();
                _projectile.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void RangedAttackEvent()
    {
        Vector3 targetPosition = _enemy.Target.transform.position;
        targetPosition.y = _enemy.transform.position.y; // Y축 고정
        _enemy.transform.LookAt(targetPosition);

        _projectile.Launch(_enemy.Target.transform, _enemy.transform.position, 1);
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
