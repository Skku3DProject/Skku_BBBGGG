using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCheckEvent : MonoBehaviour
{
    private static Collider[] _hits = new Collider[8];

    private EnemyProjectile _projectile;
    private Enemy _enemy;
    private Damage _damage;

    public List<GameObject> ProjectilePrefabs;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        _damage = new Damage(_enemy.EnemyData.DamageValue, _enemy.gameObject);
    }

    public void RangedAttackSpawn()
    {
        _projectile = null;

        if (ProjectilePrefabs == null || ProjectilePrefabs.Count == 0)
        {
            Debug.LogError("ProjectilePrefabs가 null이거나 비어있습니다!");
            return;
        }

        for (int i = 0; i < ProjectilePrefabs.Count; i++)
        {
            GameObject prefab = ProjectilePrefabs[i];

            if (prefab != null && !prefab.activeInHierarchy)
            {
                EnemyProjectile projectileComponent = prefab.GetComponent<EnemyProjectile>();
                if (projectileComponent != null)
                {
                    _projectile = projectileComponent;
                    _projectile.Initialize();
                    return; // 찾았으면 즉시 종료
                }
                else
                {
                    Debug.LogError($"발사체 {i}에 EnemyProjectile 컴포넌트가 없습니다!");
                }
            }
        }

        // 사용 가능한 발사체가 없을 때
        Debug.LogWarning(" 사용 가능한 발사체가 없습니다!");
    }

    public void RangedAttackEvent()
    {
        if (_projectile == null)
        {
            Debug.LogError("발사할 발사체가 없습니다! RangedAttackSpawn을 먼저 호출.");
            return;
        }

        if (_enemy.Target == null)
        {
            Debug.LogError("타겟이 설정되지 않았습니다!");
            return;
        }

        Vector3 targetPosition = _enemy.Target.transform.position;
        targetPosition.y = _enemy.transform.position.y; // Y축 고정
        _enemy.transform.LookAt(targetPosition);
        _projectile.Fire(_enemy.ProjectileTransfrom.position,
                         _enemy.Target.transform.position + Vector3.up * 0.5f
                        );
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

    public void CheckProjectile()
    {
        // 발사체 컴포넌트 검증
        if (ProjectilePrefabs != null && ProjectilePrefabs.Count > 0)
        {
            bool hasValidProjectile = false;
            foreach (var prefab in ProjectilePrefabs)
            {
                if (prefab != null && prefab.GetComponent<EnemyProjectile>() != null)
                {
                    hasValidProjectile = true;
                    break;
                }
            }

            if (!hasValidProjectile)
            {
                Debug.LogError("EnemyProjectile 컴포넌트가 없는 프리팹이 있습니다!");
            }
        }
        else
        {
            Debug.LogError("ProjectilePrefabs가 비어있습니다!");
        }
    }
}
