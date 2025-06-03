using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    private float _damage;
    private GameObject _owner;

    [Header("이펙트 및 설정")]
    public GameObject explosionEffect;
    public float lifetime = 10f;
    public float knockbackForce = 30f;
    public float explosionRadius = 3f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime); // 일정 시간 후 파괴
    }

    /// <summary>
    /// 외부에서 데미지와 소유자 정보를 받아 초기화
    /// </summary>
    public void Init(float damage, GameObject owner)
    {
        _damage = damage;
        _owner = owner;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<IDamageAble>(out var target))
            {
                Vector3 hitDir = (col.transform.position - transform.position).normalized;
                Damage dmg = new Damage(_damage, _owner, knockbackForce, hitDir);
                target.TakeDamage(dmg);
            }
        }

        Destroy(gameObject);
    }
}
