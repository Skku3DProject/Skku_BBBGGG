using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCheckEvent : MonoBehaviour
{
    private static Collider[] _hits = new Collider[8];


    private Enemy _enemy;
    private EnemyController _enemyController;
    private Damage _damage;
    private EnemyVisual _enemyVisual;

    // 랜덤 소환
    public float radius = 10f;
    public float minDistanceFromCenter = 2f;
    public float height = 10f;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        _enemyController = GetComponentInParent<EnemyController>();
        _enemyVisual = GetComponentInParent<EnemyVisual>();
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
        _enemy.Projectile.SetActive(true);
    }

    public void RangedAttackEvent()
    {
        _enemy.Projectile.SetActive(false);
        GameObject poolObject = EnemyObjectPoolManger.Instance.GetObject(_enemy.EnemyData.ProjectileKey, _enemy.ProjectileTransfrom.position);
        poolObject.SetActive(false);

        poolObject.TryGetComponent<EnemyProjectile>(out EnemyProjectile _projectile);
        if (_projectile == null || _enemy.Target == null)
        {
            Debug.LogError("ProjectilePrefabs가 null이거나 비어있습니다!");
            Debug.LogError("타겟이 설정되지 않았습니다!");
            return;
        }
        
        Vector3 targetPosition = _enemy.Target.transform.position;
        targetPosition.y = _enemy.transform.position.y; // Y축 고정
        _enemy.transform.LookAt(targetPosition);

        _projectile.Fire(_enemy.Target.transform.position + Vector3.up * 0.5f);
    }
    // 병사 되살리기 공격
    public void Summon()
    {
        List<Vector3> positions = new List<Vector3>(EnemyManager.Instance.SpawnPositionList);
        List<Enemy> enemies = new List<Enemy>(EnemyManager.Instance.SummonEnemies);
        int index = 0;
        foreach (Enemy enemy in enemies)
        {
            Vector3 position = positions[index++];
            position.y = _enemy.ProjectileTransfrom.position.y; // 예: 머리 위 2미터
            EnemyPoolManager.Instance.GetObject(enemy.EnemyData.Key, position);

        }

        EnemyManager.Instance.SummonEnemiesClear();
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
            // 풀링 소환
            EnemyObjectPoolManger.Instance.GetObject(_enemy.EnemyData.AreaObjectKey, spawnPosition);
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

    public void Self_DestructAttackEvent()
    {
        // 달려가다가 어택 상태가 되면 3번 발동후 펑 
        StartCoroutine(Self_DestructAttackCroutine());
    }

    private IEnumerator Self_DestructAttackCroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            _enemyVisual.PlayHitFeedback(_enemy.EnemyData.DamagedTime);
            yield return new WaitForSeconds(_enemy.EnemyData.Self_DestructTime);
        }
        BlockSystem.DamageBlocksInRadius(_enemy.transform.position, _enemy.EnemyData.AreaRange, (int)_damage.Value);
        _enemyController.EndDieAnimEvent();

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
        // 폭발 이팩트

        yield break;
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
