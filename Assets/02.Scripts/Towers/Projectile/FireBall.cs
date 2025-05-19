using UnityEngine;

public class FireBall : ProjectileBase
{
    [SerializeField] private float damageRadius = 3f;
    [SerializeField] private LayerMask targetMask;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player")) // Ʈ���� ������ Ȯ�ο�
        {
            Debug.Log("���� ������ �߻�");

            // �ֺ� ���� �� ��� Ž��
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
