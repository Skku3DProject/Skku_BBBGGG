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
    public float explosionRadius = 2f;
    public float explosionForce = 50f;

    [Header("차징 화살")]
    public GameObject chargeImpactEffect;
    public float chargeImpactRadius = 2f;
    public float chargeImpactForce = 25f;

    [Header("관통 설정")]
    public int maxPierceCount = 10;
    private int currentPierceCount = 0;
    private bool isPiercing = false;

    [Header("사운드")]
    [SerializeField] private AudioClip normalHitSound;
    [SerializeField] private AudioClip explosiveHitSound;
    [SerializeField] private AudioClip chargingHitSound;

    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _audioSource.spatialBlend = 1f;
        _audioSource.minDistance = 5f;
        _audioSource.maxDistance = 100f;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
    }

    private void FixedUpdate()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);
            transform.Rotate(90f, 0f, 0f, Space.Self);
        }

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
        if (target.CompareTag("BaseTower"))
            return;

        if (HitVfx != null)
            Instantiate(HitVfx, transform.position, Quaternion.identity);

        if (target.TryGetComponent<IDamageAble>(out var d))
        {
            d.TakeDamage(new Damage(_damage, _owner, 10f));
        }

        PlayHitSound();
    }

    private void ApplyChargingDamage()
    {
        if (chargeImpactEffect != null)
            Instantiate(chargeImpactEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, chargeImpactRadius);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("BaseTower"))
                continue;

            if (hit.TryGetComponent<IDamageAble>(out var d))
            {
                Vector3 hitDir = (hit.transform.position - transform.position).normalized;
                d.TakeDamage(new Damage(_damage, _owner, chargeImpactForce, hitDir));
            }
        }

        BlockManager.Instance.DamageBlocksInRadius(transform.position, chargeImpactRadius, 10);
        PlayHitSound();
    }

    private void Explode()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("BaseTower"))
                continue;

            if (hit.TryGetComponent<IDamageAble>(out var d))
            {
                Vector3 hitDir = (hit.transform.position - transform.position).normalized;
                d.TakeDamage(new Damage(_damage, _owner, explosionForce, hitDir));
            }
        }

        BlockManager.Instance.DamageBlocksInRadius(transform.position, explosionRadius, 10);
        PlayHitSound();
    }

    private bool CheckGroundHit()
    {
        if (_rb == null || _rb.linearVelocity.sqrMagnitude < 0.1f) return false;

        Vector3 forwardDir = _rb.linearVelocity.normalized;
        Vector3 downForward = (forwardDir + Vector3.down).normalized;
        float rayLength = 0.7f;

        return Physics.Raycast(transform.position, downForward, rayLength, LayerMask.GetMask("Ground"));
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

        // 사운드 길이만큼 기다리고 삭제
        // Destroy(gameObject);
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

    private void PlayHitSound()
    {
        if (_audioSource == null) return;

        AudioClip clipToPlay = null;

        switch (_arrowType)
        {
            case ArrowType.Normal:
                clipToPlay = normalHitSound;
                break;
            case ArrowType.Explosive:
                clipToPlay = explosiveHitSound;
                break;
            case ArrowType.Charging:
                clipToPlay = chargingHitSound;
                break;
        }

        if (clipToPlay != null)
        {
            _audioSource.clip = clipToPlay;
            _audioSource.Play();

            DestroyWithSound(clipToPlay);
        }
    }

    private void DestroyWithSound(AudioClip clipToPlay)
    {
        GetComponent<Collider>().enabled = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = true;
        enabled = false;

        if (TryGetComponent<Renderer>(out var renderer))
        {
            renderer.enabled = false;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        Destroy(gameObject, clipToPlay.length);
    }
}
