using System.Linq;
using UnityEngine;

public class FireBall : ProjectileBase
{
    [SerializeField] private LayerMask targetMask;

    protected override void OnGroundHit(RaycastHit hit)
    {
        base.OnGroundHit(hit);

        //데미지 추가기능

        // 주변 데미지 처리
        Collider[] hits = Physics.OverlapSphere(hit.point, _data.SplashRadius, targetMask);
        Debug.Log(_data.SplashRadius);
        DrawDebugSphere(hit.point, _data.SplashRadius);
        if (hits.Length > 0)
            Debug.Log("Enemy SplashDamaged");
        foreach (var col in hits)
        {
            Debug.Log("GroundHit SplashDamage");
            if (col.TryGetComponent<IDamageAble>(out var target))
            {
                Debug.Log("GroundHit SplashDamage");
                target.TakeDamage(new Damage(_data.Damage, gameObject,5));
            }
        }
        BlockSystem.DamageBlocksInRadius(hit.point, 5, 10);

        //Vector3Int blockPos = Vector3Int.FloorToInt(hit.point);
        //BlockSystem.DamageBlock(blockPos, 10);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Enemy")) // 트리거 시작점 확인용
        {

            // 주변 범위 내 대상 탐색
            Collider[] hits = Physics.OverlapSphere(other.transform.position, _data.SplashRadius, targetMask);
            Debug.Log(_data.SplashRadius);
            DrawDebugSphere(other.transform.position, _data.SplashRadius);
            if (hits.Length>0) 
                Debug.Log("Enemy SplashDamaged");
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageAble>(out var d))
                {
                    Debug.Log("Enemy SplashDamage");
                    d.TakeDamage(new Damage(_data.Damage,gameObject,5));
                    BlockSystem.DamageBlocksInRadius(hit.transform.position, 5, 10);
                }
            }
        }

        if (HitVfxPrefab)
        {
            ObjectPool.Instance.GetObject(HitVfxPrefab,other.transform.position,other.transform.rotation);
        }

        ObjectPool.Instance.ReturnToPool(gameObject);
    }

    void DrawDebugSphere(Vector3 center, float radius, int segments = 12)
    {
        for (int i = 0; i < segments; i++)
        {
            float theta1 = (i * Mathf.PI * 2f) / segments;
            float theta2 = ((i + 1) * Mathf.PI * 2f) / segments;

            // XY 평면 원
            Vector3 p1 = center + new Vector3(Mathf.Cos(theta1), Mathf.Sin(theta1), 0) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(theta2), Mathf.Sin(theta2), 0) * radius;
            Debug.DrawLine(p1, p2, Color.red, 1f);

            // XZ 평면 원
            Vector3 q1 = center + new Vector3(Mathf.Cos(theta1), 0, Mathf.Sin(theta1)) * radius;
            Vector3 q2 = center + new Vector3(Mathf.Cos(theta2), 0, Mathf.Sin(theta2)) * radius;
            Debug.DrawLine(q1, q2, Color.red, 1f);

            // YZ 평면 원
            Vector3 r1 = center + new Vector3(0, Mathf.Cos(theta1), Mathf.Sin(theta1)) * radius;
            Vector3 r2 = center + new Vector3(0, Mathf.Cos(theta2), Mathf.Sin(theta2)) * radius;
            Debug.DrawLine(r1, r2, Color.red, 1f);
        }
    }
}
