using UnityEngine;
public enum ArrowType
{
    Normal,
    Explosive,
    Charging
}
public class PlayerArrow : MonoBehaviour
{
    private float _damage;
    private Rigidbody _rb;
    private ArrowType _arrowType;
    private GameObject _owner;

    [Header("Æø¹ß ÀÌÆåÆ®")]
    public GameObject explosionEffect;
    public float explosionRadius = 3f;
    public float explosionForce = 500f;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);

            transform.Rotate(90f, 0f, 0f, Space.Self);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
        {

            switch (_arrowType)
            {
                case ArrowType.Normal:
                    ApplyNormalDamage(collision);
                    break;

                case ArrowType.Explosive:
                    Explode();
                    break;
            }

            Destroy(gameObject);

        }
    }
    private void ApplyNormalDamage(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageAble>(out var d))
        {
            d.TakeDamage(new Damage(_damage, gameObject, 10f));
        }
    }
    private void Explode()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent<IDamageAble>(out var d))
            {
                d.TakeDamage(new Damage(_damage, gameObject, explosionForce));
            }
        }
        BlockSystem.DamageBlocksInRadius(transform.position, explosionRadius, 10);

    }

    public void ArrowInit(float Damage, ArrowType type, GameObject owner)
    {
        _damage = Damage;
        _arrowType = type;
        _owner = owner;
    }
}