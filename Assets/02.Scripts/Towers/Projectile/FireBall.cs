using UnityEngine;

public class FireBall : ProjectileBase
{
    [SerializeField] private LayerMask targetMask;

    protected override void OnGroundHit(RaycastHit hit)
    {
        base.OnGroundHit(hit);

        //������ �߰����

        // �ֺ� ������ ó��
        Collider[] targets = Physics.OverlapSphere(hit.point, _data.SplashRadius, targetMask);
        foreach (var col in targets)
        {
            if (col.TryGetComponent<DamageAble>(out var target))
            {
                //target.TakeDamage(damage);
            }
        }
        BlockSystem.DamageBlocksInRadius(hit.point, 3, 10);

        //Vector3Int blockPos = Vector3Int.FloorToInt(hit.point);
        //BlockSystem.DamageBlock(blockPos, 10);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player")) // Ʈ���� ������ Ȯ�ο�
        {

            // �ֺ� ���� �� ��� Ž��
            Collider[] hits = Physics.OverlapSphere(transform.position, _data.SplashRadius, targetMask);
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
            ObjectPool.Instance.GetObject(HitVfxPrefab,other.transform.position,other.transform.rotation);
            //Instantiate(HitVfxPrefab, other.transform);
        }

        ObjectPool.Instance.ReturnToPool(gameObject);
        //Destroy(gameObject);
    }
}
