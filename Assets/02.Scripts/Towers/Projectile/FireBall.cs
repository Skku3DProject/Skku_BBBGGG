using UnityEngine;

public class FireBall : ProjectileBase
{
    [SerializeField] private float damageRadius = 3f;
    [SerializeField] private LayerMask targetMask;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player")) // 트리거 시작점 확인용
        {
            Debug.Log("범위 데미지 발생");

            // 주변 범위 내 대상 탐색
            Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius, targetMask);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<DamageAble>(out var d))
                {
                    //d.TakeDamage(damage);
                }
            }
        }

        if (HitVfxPrefab)
        {
            Instantiate(HitVfxPrefab, other.transform);
        }

        Destroy(gameObject);
    }
}
