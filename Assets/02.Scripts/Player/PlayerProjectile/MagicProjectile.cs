using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    private float _damage;
    private GameObject _owner;

    [Header("이펙트 및 설정")]
    public GameObject hitEffect;
    public float lifetime = 5f;
    public float knockbackForce = 15f;


    private Rigidbody _rb;
    private void Awake()
    {
        Destroy(gameObject, lifetime); // 일정 시간 뒤 자동 삭제
    }

    /// <summary>
    /// WandAttack에서 발사 시 호출됨
    /// </summary>
    public void Init(float damage, GameObject owner)
    {
        _damage = damage;
        _owner = owner;

        if (_rb != null)
        {
            _rb.useGravity = false;                  // 중력 무시
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 고속 충돌 안정화
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 히트 이펙트 재생
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // 범위 데미지
        float radius = 1f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<IDamageAble>(out var target))
            {
                Vector3 hitDir = (col.transform.position - transform.position).normalized;
                Damage dmg = new Damage(_damage, _owner, knockbackForce, hitDir);
                target.TakeDamage(dmg);
            }
        }

        Destroy(gameObject); // 투사체 제거
    }
}
