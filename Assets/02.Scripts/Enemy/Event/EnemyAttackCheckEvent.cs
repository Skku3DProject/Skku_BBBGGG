using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCheckEvent : MonoBehaviour
{
    private static Collider[] _hits = new Collider[8];

    private EnemyProjectile _projectile;
    private Enemy _enemy;
    private EnemyController _enemyController;
    private Damage _damage;

    // ���� ��ȯ
    public float radius = 10f;
    public float minDistanceFromCenter = 2f;
    public float height = 10f;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        _enemyController = GetComponentInParent<EnemyController>();
        _damage = new Damage(_enemy.EnemyData.DamageValue, _enemy.gameObject);
    }

    public void TransformForwardAttackStart()
    {
        StartCoroutine(TransformForwardAttack_Coroutine());
    }

    public void TransformForwardAttackStop()
    {
        StopCoroutine(TransformForwardAttack_Coroutine());
    }

    private IEnumerator TransformForwardAttack_Coroutine()
    {
        _enemy.CharacterController.Move(transform.forward * (_enemy.EnemyData.Speed * 2) * Time.deltaTime);
        yield return null;
    }

    public void RangedAttackSpawn()
    {
        GameObject poolObject = EnemyObjectPoolManger.Instance.GetObject(_enemy.EnemyData.ProjectileKey, _enemy.ProjectileTransfrom.position);
        _enemyController.IsAttack = true;
        poolObject.TryGetComponent<EnemyProjectile>(out _projectile);
        if (_projectile == null)
        {
            Debug.LogError("ProjectilePrefabs�� null�̰ų� ����ֽ��ϴ�!");
            return;
        }
        _projectile.transform.SetParent(_enemy.ProjectileTransfrom);
    }

    public void RangedAttackEvent()
    {
        if (_projectile == null)
        {
           // Debug.LogError("�߻��� �߻�ü�� �����ϴ�! RangedAttackSpawn�� ���� ȣ��.");
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

        _projectile.Fire(//_enemy.ProjectileTransfrom.position,
                         _enemy.Target.transform.position + Vector3.up * 0.5f
                        );

        _enemyController.IsAttack = false;
    }
    // ���� �ǻ츮�� ����
    public void Summon()
    {
        int currentEnemisCount = EnemyManager.Instance.FirstActiveEnemies.Count - EnemyManager.Instance.ActiveEnemies.Count;
        List<Vector3> positions = new List<Vector3>();
        List<string> keys = new List<string>(currentEnemisCount);
        foreach (var enemy in EnemyManager.Instance.FirstActiveEnemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                keys.Add(enemy.EnemyData.Key);
                int index = EnemyManager.Instance.FirstActiveEnemies.IndexOf(enemy);
                if (index < EnemyManager.Instance.SpawnPositionList.Count)
                    positions.Add(EnemyManager.Instance.SpawnPositionList[index]);
                else
                    Debug.LogWarning($"Index {index} out of bounds for SpawnPositionList");
            }
        }

        for (int i=0; i< currentEnemisCount; i++)
        {
            GameObject enemy = EnemyPoolManager.Instance.GetObject(keys[i]);
            Vector3 position = positions[i];
            position.y = transform.position.y + _enemy.ProjectileTransfrom.position.y; // ��: �Ӹ� �� 2����
            enemy.transform.position = position;
        }

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

    public void SpawnAreaAttack()
    {
        for (int i = 0; i < _enemy.EnemyData.MaxAreaAttackCount; i++)
        {
            Vector3 randomPos = GetRandomPositionAround();
            Vector3 spawnPosition = new Vector3(randomPos.x, transform.position.y + height, randomPos.z);
            // Ǯ�� ��ȯ
            EnemyObjectPoolManger.Instance.GetObject(_enemy.EnemyData.AreaObjectKey,spawnPosition);
        }
    }

    public void AreaAttackEventStart()
    {
        StartCoroutine(AreaAttack_Coroutine());
    }

    public void AreaAttackEventEnd()
    {
        StopCoroutine(AreaAttack_Coroutine());
    }

    private IEnumerator AreaAttack_Coroutine()
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
        yield return null;
    }

    public void EndAttackEnvet()
    {
        _enemyController.IsAttack = false;
    }

    private Vector3 GetRandomPositionAround()
    {
        Vector3 result;
        do
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float distance = Random.Range(minDistanceFromCenter, radius);
            float x = Mathf.Cos(angle) * distance;
            float z = Mathf.Sin(angle) * distance;
            result = new Vector3(transform.position.x + x, 0f, transform.position.z + z);
        }
        while ((new Vector2(result.x, result.z) - new Vector2(transform.position.x, transform.position.z)).magnitude < minDistanceFromCenter);

        return result;
    }

}
