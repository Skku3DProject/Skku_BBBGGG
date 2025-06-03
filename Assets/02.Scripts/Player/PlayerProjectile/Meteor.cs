using UnityEngine;

public class Meteor : MonoBehaviour
{
    private float _damage;
    private GameObject _owner;
    private Transform _target;

    [Header("이펙트 및 설정")]
    public GameObject explosionEffect;
    public float lifetime = 10f;
    public float knockbackForce = 30f;
    public float explosionRadius = 3f;
    public float trackingSpeed = 30f;
    public float turnSpeed = 5f;

    [Header("사운드")]
    public AudioClip explosionSound;
    public AudioSource audioSource;

    private Rigidbody _rb;
    private bool _exploded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime); // 일정 시간 후 파괴

        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f;                // 3D 사운드
            audioSource.minDistance = 5f;
            audioSource.maxDistance = 30f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    public void Init(float damage, GameObject owner, Transform target = null)
    {
        _damage = damage;
        _owner = owner;
        _target = target;

        if (_rb != null)
        {
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if (_target == null)
        {
            // 타겟 없으면 즉시 폭발
            Explode();
        }
    }

    private void Update()
    {
        if (_exploded) return;

        // 타겟 추적
        if (_target != null)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
            transform.position += transform.forward * trackingSpeed * Time.deltaTime;
        }

        // 바닥과 Raycast 충돌 감지
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f, LayerMask.GetMask("Ground")))
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_exploded) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        _exploded = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        if (explosionSound != null)
        {
            audioSource.clip = explosionSound;
            audioSource.Play();
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
        Destroy(gameObject, explosionSound.length); 
    }
}

