using UnityEngine;

public class Arrow : ProjectileBase
{

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        Debug.Log("Hit Player");
        if (other.TryGetComponent<DamageAble>(out var d))
        {
            Debug.Log("Hit Player");
            //d.TakeDamage(damage);

        }
        if (HitVfxPrefab)
        {
            Instantiate(HitVfxPrefab, other.transform);
        }
        Destroy(gameObject);
    }
}
