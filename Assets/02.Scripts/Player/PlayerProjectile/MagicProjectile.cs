using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    private float _damage;
    private GameObject _owner;

    [Header("����Ʈ �� ����")]
    public GameObject hitEffect;
    public float lifetime = 5f;
    public float knockbackForce = 15f;


    private Rigidbody _rb;
    private void Awake()
    {
        Destroy(gameObject, lifetime); // ���� �ð� �� �ڵ� ����
    }

    /// <summary>
    /// WandAttack���� �߻� �� ȣ���
    /// </summary>
    public void Init(float damage, GameObject owner)
    {
        _damage = damage;
        _owner = owner;

        if (_rb != null)
        {
            _rb.useGravity = false;                  // �߷� ����
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // ��� �浹 ����ȭ
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ��Ʈ ����Ʈ ���
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // ���� ������
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

        Destroy(gameObject); // ����ü ����
    }
}
