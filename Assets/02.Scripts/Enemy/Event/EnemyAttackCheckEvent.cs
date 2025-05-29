using System.Collections;
using UnityEngine;

public class EnemyAttackCheckEvent : MonoBehaviour
{
    private static Collider[] _hits = new Collider[8];

    private EnemyProjectile _projectile;
    private Enemy _enemy;
    private Damage _damage;

    private bool _isAttack = false;
    public bool IsAttack => _isAttack;

    // 랜덤 소환
    public float radius = 10f;
    public float minDistanceFromCenter = 2f;
    public float height = 10f;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
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
        _isAttack = true;
        poolObject.TryGetComponent<EnemyProjectile>(out _projectile);
        if (_projectile == null)
        {
            Debug.LogError("ProjectilePrefabs가 null이거나 비어있습니다!");
            return;
        }
        _projectile.transform.SetParent(_enemy.ProjectileTransfrom);
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

        _projectile.Fire(//_enemy.ProjectileTransfrom.position,
                         _enemy.Target.transform.position + Vector3.up * 0.5f
                        );

        _isAttack = false;
    }
    // 병사 되살리기 공격
    public void Summon()
    {
        //for(int i = EnemyManager.Instance.ActiveEnemies.Count; i < EnemyManager.Instance.CurrentStageSpawnCount; i ++)
        //{
        //    Vector3 randomPos = GetRandomPositionAround();
        //    Vector3 spawnPosition = new Vector3(randomPos.x, transform.position.y + height, randomPos.z);
        //    // 죽을때마다 담는다?
        //    EnemyPoolManager.Instance.GetObject()
        //}
        //for (int i = 0; i < _enemy.EnemyData.PrefabSize; i++)
        //{
        //    Vector3 randomPos = GetRandomPositionAroundPlayer();
        //    Vector3 spawnPosition = new Vector3(randomPos.x, transform.position.y + height, randomPos.z);
        //    // 풀링 소환
        //    //Instantiate(fallingObjectPrefab, spawnPosition, Quaternion.identity);
        //}
    }
    public void MeleeAttackEvent()
    {
        _isAttack = true;
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
        //for (int i = 0; i < _enemy.EnemyData.PrefabSize; i++)
        //{
        //    Vector3 randomPos = GetRandomPositionAroundPlayer();
        //    Vector3 spawnPosition = new Vector3(randomPos.x, transform.position.y + height, randomPos.z);
        //    // 풀링 소환
        //    //Instantiate(fallingObjectPrefab, spawnPosition, Quaternion.identity);
        //}
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
        _isAttack = false;
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
