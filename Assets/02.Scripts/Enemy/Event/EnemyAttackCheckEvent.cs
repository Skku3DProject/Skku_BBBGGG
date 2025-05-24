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
            Debug.LogError("ProjectilePrefabs�� null�̰ų� ����ֽ��ϴ�!");
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
                    return; // ã������ ��� ����
                }
                else
                {
                    Debug.LogError($"�߻�ü {i}�� EnemyProjectile ������Ʈ�� �����ϴ�!");
                }
            }
        }

        // ��� ������ �߻�ü�� ���� ��
        Debug.LogWarning(" ��� ������ �߻�ü�� �����ϴ�!");
    }

    public void RangedAttackEvent()
    {
        if (_projectile == null)
        {
            Debug.LogError("�߻��� �߻�ü�� �����ϴ�! RangedAttackSpawn�� ���� ȣ��.");
            return;
        }

        if (_enemy.Target == null)
        {
            Debug.LogError("Ÿ���� �������� �ʾҽ��ϴ�!");
            return;
        }

        Vector3 targetPosition = _enemy.Target.transform.position;
        targetPosition.y = _enemy.transform.position.y; // Y�� ����
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
        // �߻�ü ������Ʈ ����
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
                Debug.LogError("EnemyProjectile ������Ʈ�� ���� �������� �ֽ��ϴ�!");
            }
        }
        else
        {
            Debug.LogError("ProjectilePrefabs�� ����ֽ��ϴ�!");
        }
    }
}
