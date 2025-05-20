using UnityEngine;

public class FireBall : ProjectileBase
{
    [SerializeField] private LayerMask targetMask;

    protected override void OnGroundHit(RaycastHit hit)
    {
        base.OnGroundHit(hit);

        //데미지 추가기능

        // 주변 데미지 처리
        Collider[] targets = Physics.OverlapSphere(hit.point, _data.SplashRadius, targetMask);
        foreach (var col in targets)
        {
            Debug.Log("aaa");
            if (col.TryGetComponent<IDamageAble>(out var target))
            {
                target.TakeDamage(new Damage(_data.Damage, gameObject));
            }
        }
        //BlockSystem.DamageBlocksInRadius(hit.point, 3, 10);

        //Vector3Int blockPos = Vector3Int.FloorToInt(hit.point);
        //BlockSystem.DamageBlock(blockPos, 10);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Enemy")) // 트리거 시작점 확인용
        {

            // 주변 범위 내 대상 탐색
            Collider[] hits = Physics.OverlapSphere(transform.position, _data.SplashRadius, targetMask);
            foreach (var hit in hits)
            {
                Debug.Log("aaa");
                if (hit.TryGetComponent<IDamageAble>(out var d))
                {
                    d.TakeDamage(new Damage(_data.Damage,gameObject));
                }
            }
        }

        if (HitVfxPrefab)
        {
            ObjectPool.Instance.GetObject(HitVfxPrefab,other.transform.position,other.transform.rotation);
            //Instantiate(HitVfxPrefab, other.transform);
        }

        ObjectPool.Instance.ReturnToPool(gameObject);
        //Destroy(gameObject);
    }
}
