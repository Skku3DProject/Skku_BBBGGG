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

    public GameObject[] ArrowVfx;
    [Header("�Ϲ�ȭ��")]
    public GameObject HitVfx;

    [Header("���� ȭ��")]
    public GameObject explosionEffect;
    public float explosionRadius = 3f;
    public float explosionForce = 50f;
    [Header("��¡ ȭ��")]
    public GameObject chargeImpactEffect;
    public float chargeImpactRadius = 1.5f;
    public float chargeImpactForce = 25f;
    [Header("���� ����")]
    public int maxPierceCount = 10; // �ִ� ���� ���� Ƚ��
    private int currentPierceCount = 0;
    private bool isPiercing = false;


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
        bool hitEnemy = collision.gameObject.CompareTag("Enemy");
        bool hitGround = collision.gameObject.CompareTag("Ground");

        if (_arrowType == ArrowType.Charging && isPiercing)
        {
            if (hitEnemy)
            {
                ApplyChargingDamage(collision);
                currentPierceCount++;

                if (currentPierceCount >= maxPierceCount)
                {
                    Destroy(gameObject); // ���� Ƚ�� �ʰ� �� ����
                }

                return; // ���� �����ϹǷ� �ı����� ����
            }
            else if (hitGround)
            {
                if (chargeImpactEffect != null)
                    Instantiate(chargeImpactEffect, transform.position, Quaternion.identity);

                BlockSystem.DamageBlocksInRadius(transform.position, chargeImpactRadius, 10);
                Destroy(gameObject); // ���̳� ���� �ε����� �ı�
            }
        }
        else
        {
            switch (_arrowType)
            {
                case ArrowType.Normal:
                    ApplyNormalDamage(collision);
                    break;
                case ArrowType.Explosive:
                    Explode();
                    break;
                case ArrowType.Charging:
                    ApplyChargingDamage(collision); // ���� ������ ���
                    break;
            }

            Destroy(gameObject);
        }
    }
    private void ApplyChargingDamage(Collision collision)
    {
        // ����Ʈ ���
        if (chargeImpactEffect != null)
            Instantiate(chargeImpactEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, chargeImpactRadius);

        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent<IDamageAble>(out var d))
            {
                Vector3 hitDirection = (hit.transform.position - transform.position).normalized;
                d.TakeDamage(new Damage(_damage, _owner, chargeImpactForce));
            }
        }

        // ������ ��� �� �� �ִٸ� (����)
        BlockSystem.DamageBlocksInRadius(transform.position, chargeImpactRadius, 10);
    }

    private void ApplyNormalDamage(Collision collision)
    {
        if (HitVfx != null)
            Instantiate(HitVfx, transform.position, Quaternion.identity);


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

        if (type == ArrowType.Charging)
        {
            isPiercing = true;
            currentPierceCount = 0;
            _rb.useGravity = false;

            ArrowVfx[(int)type].SetActive(true);
        }
        else if(type == ArrowType.Explosive)
        {
            ArrowVfx[(int)type].SetActive(true);
        }
        else if(type == ArrowType.Normal)
        {
            ArrowVfx[(int)type].SetActive(true);
        }
    }
}