using UnityEngine;

public class Arrow : ProjectileBase
{
    protected override void OnGroundHit(RaycastHit hit)
    {
        base.OnGroundHit(hit);

        Vector3Int blockPos = Vector3Int.FloorToInt(hit.point+hit.normal*-0.5f);
        BlockSystem.DamageBlock(blockPos, 10);
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.TryGetComponent<DamageAble>(out var d))
        {
            //d.TakeDamage(damage);

        }
        if (HitVfxPrefab)
        {
            ObjectPool.Instance.GetObject(HitVfxPrefab, other.transform.position, other.transform.rotation);
            //Instantiate(HitVfxPrefab, other.transform);
        }
        ObjectPool.Instance.ReturnToPool(gameObject);//Destroy(gameObject);
    }
}
