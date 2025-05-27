using System.Drawing;
using System.Linq;
using UnityEngine;

public class FireBall : ProjectileBase
{
    [SerializeField] private LayerMask targetMask;

    protected override void OnGroundHit(RaycastHit hit)
    {
        base.OnGroundHit(hit);

        //������ �߰����

        // �ֺ� ������ ó��
        Collider[] hits = Physics.OverlapSphere(hit.point, _data.SplashRadius, targetMask);
        Debug.Log(_data.SplashRadius);
        if (hits.Length > 0)
            Debug.Log("Enemy SplashDamaged");
        foreach (var col in hits)
        {
            Debug.Log("GroundHit SplashDamage");
            if (col.TryGetComponent<IDamageAble>(out var target))
            {
                Debug.Log("GroundHit SplashDamage");
                target.TakeDamage(new Damage(_data.Damage, _owner, 5));
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Enemy")) // Ʈ���� ������ Ȯ�ο�
        {

            // �ֺ� ���� �� ��� Ž��
            Collider[] hits = Physics.OverlapSphere(other.transform.position, _data.SplashRadius, targetMask);

            foreach (var hit in hits)
            {
                Debug.Log(hit.gameObject.name);
                if (hit.TryGetComponent<IDamageAble>(out var d))
                {
                    Debug.Log("Enemy SplashDamage2");
                    d.TakeDamage(new Damage(_data.Damage, _owner, 5));
                }
            }

        }

        if (HitVfxPrefab)
        {
            ObjectPool.Instance.GetObject(HitVfxPrefab, other.transform.position, other.transform.rotation);
        }

        ObjectPool.Instance.ReturnToPool(gameObject);
    }

}
