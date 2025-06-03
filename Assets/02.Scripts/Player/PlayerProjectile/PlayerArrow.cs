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

    [Header("일반화살")]
    public GameObject HitVfx;

    [Header("폭발 화살")]
    public GameObject explosionEffect;
    public float explosionRadius = 3f;
    public float explosionForce = 50f;

    [Header("차징 화살")]
    public GameObject chargeImpactEffect;
    public float chargeImpactRadius = 1.5f;
    public float chargeImpactForce = 25f;

    [Header("관통 설정")]
    public int maxPierceCount = 10;
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

        // 바닥 충돌 감지
        if (CheckGroundHit())
        {
            HandleGroundImpact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isEnemy = other.CompareTag("Enemy");

        if (_arrowType == ArrowType.Charging && isPiercing)
        {
            if (isEnemy)
            {
                ApplyChargingDamage();
                currentPierceCount++;

                if (currentPierceCount >= maxPierceCount)
                    Destroy(gameObject);

                return;
            }
            else if (CheckGroundHit())
            {
                HandleGroundImpact();
                Destroy(gameObject);
            }
        }
        else
        {
            if (isEnemy || CheckGroundHit())
            {
                switch (_arrowType)
                {
                    case ArrowType.Normal:
                        ApplyNormalDamage(other);
                        break;
                    case ArrowType.Explosive:
                        Explode();
                        break;
                    case ArrowType.Charging:
                        ApplyChargingDamage();
                        break;
                }

                Destroy(gameObject);
            }
        }
    }

    private void ApplyNormalDamage(Collider target)
    {
        if (HitVfx != null)
            Instantiate(HitVfx, transform.position, Quaternion.identity);

        if (target.TryGetComponent<IDamageAble>(out var d))
        {
            d.TakeDamage(new Damage(_damage, _owner, 10f));
        }
    }

    private void ApplyChargingDamage()
    {
        if (chargeImpactEffect != null)
            Instantiate(chargeImpactEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, chargeImpactRadius);
        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent<IDamageAble>(out var d))
            {
                Vector3 hitDir = (hit.transform.position - transform.position).normalized;
                d.TakeDamage(new Damage(_damage, _owner, chargeImpactForce, hitDir));
            }
        }

        BlockSystem.DamageBlocksInRadius(transform.position, chargeImpactRadius, 10);
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
                Vector3 hitDir = (hit.transform.position - transform.position).normalized;
                d.TakeDamage(new Damage(_damage, _owner, explosionForce, hitDir));
            }
        }

        BlockSystem.DamageBlocksInRadius(transform.position, explosionRadius, 10);
    }

    private bool CheckGroundHit()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.5f, LayerMask.GetMask("Ground"));
    }

    private void HandleGroundImpact()
    {
        switch (_arrowType)
        {
            case ArrowType.Normal:
                if (HitVfx != null)
                    Instantiate(HitVfx, transform.position, Quaternion.identity);
                break;
            case ArrowType.Charging:
                ApplyChargingDamage();
                break;
            case ArrowType.Explosive:
                Explode();
                break;
        }

        Destroy(gameObject);
    }

    public void ArrowInit(float damage, ArrowType type, GameObject owner)
    {
        _damage = damage;
        _arrowType = type;
        _owner = owner;

        if (type == ArrowType.Charging)
        {
            isPiercing = true;
            currentPierceCount = 0;
            _rb.useGravity = false;
        }
        else
        {
            _rb.useGravity = true;
        }

        ArrowVfx[(int)type].SetActive(true);
    }
}