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
    public int maxPierceCount = 10; // 최대 관통 가능 횟수
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
                    Destroy(gameObject); // 관통 횟수 초과 시 제거
                }

                return; // 적을 관통하므로 파괴하지 않음
            }
            else if (hitGround)
            {
                if (chargeImpactEffect != null)
                    Instantiate(chargeImpactEffect, transform.position, Quaternion.identity);

                BlockSystem.DamageBlocksInRadius(transform.position, chargeImpactRadius, 10);
                Destroy(gameObject); // 벽이나 땅에 부딪히면 파괴
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
                    ApplyChargingDamage(collision); // 관통 꺼졌을 경우
                    break;
            }

            Destroy(gameObject);
        }
    }
    private void ApplyChargingDamage(Collision collision)
    {
        // 이펙트 재생
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

        // 블럭에도 충격 줄 수 있다면 (선택)
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