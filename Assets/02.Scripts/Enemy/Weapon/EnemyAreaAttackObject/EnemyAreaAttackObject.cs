using UnityEngine;

public class EnemyAreaAttackObject : MonoBehaviour, IEnemyPoolable
{
    public So_EnemyProjectile ProjectileData;

    private TrailRenderer _trailRenderer;
    private Damage _damage;
    private Rigidbody rb;
    private float _timer = 0;
    private bool _isPooled = false; // 풀 반환 상태 추적

    private Collider[] hits = new Collider[100];

    public void OnSpawn()
    {
        _isPooled = false;
        Initialize();
    }
    public void OnDespawn()
    {
        transform.SetParent(EnemyObjectPoolManger.Instance.gameObject.transform);
        _isPooled = true;
    }
    private void Awake()
    {
        if (ProjectileData != null)
        {
            _damage = new Damage(ProjectileData.Damage, this.gameObject, ProjectileData.KnockbackPower);
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
            rb = GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError($"ProjectileData is null on {gameObject.name}");
        }
    }

    public void Initialize()
    {
        _trailRenderer.Clear();
        _trailRenderer.enabled = false;
        _isPooled = false;
        transform.SetParent(null);
        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= ProjectileData.LostTime)
        {
            UnEnable();
            _timer = 0;
        }

        CheckGroundHit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isPooled || other == null || other.CompareTag("Enemy")) return;

        if (ProjectileData.Type == EnemyProjectileType.Area)
        {
            AreaAttack(other);
        }
        else if (ProjectileData.Type == EnemyProjectileType.Point)
        {
            PointAttack(other);
        }
    }

    private bool CheckGroundHit()
    {
        if (_isPooled || ProjectileData == null) return false;

        Vector3 down = Vector3.down;

        if (Physics.Raycast(transform.position, down, out RaycastHit hit, 0.1f, ProjectileData.GroundMask))
        {
            OnGroundHit(hit);
            UnEnable();
            return true; // 충돌 성공
        }
        return false; // 충돌 못 함
    }
    private void OnGroundHit(RaycastHit hit)
    {
        if (_isPooled) return;

        Vector3Int blockPos = Vector3Int.FloorToInt(hit.point + hit.normal * -0.5f);
        BlockSystem.DamageBlocksInRadius(blockPos, ProjectileData.AreaRange, (int)_damage.Value);
    }

    private void AreaAttack(Collider other)
    {
        BlockSystem.DamageBlocksInRadius(other.transform.position, ProjectileData.AreaRange, (int)_damage.Value);

        int cnt = Physics.OverlapSphereNonAlloc(
            transform.position,
            ProjectileData.AreaRange,
            hits
        );

        for (int i = 0; i < cnt; i++)
        {
            if (hits[i].TryGetComponent<IDamageAble>(out var dmg))
            {
                dmg.TakeDamage(_damage);
            }
        }

    }

    private void PointAttack(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out IDamageAble damageAble))
        {
            if (damageAble != null && _damage != null)
            {
                damageAble.TakeDamage(_damage);
            }
        }

        UnEnable();
    }


    private void UnEnable()
    {
        if (_isPooled) return;

        // EnemyObjectPoolManger 인스턴스 확인
        if (EnemyObjectPoolManger.Instance != null && ProjectileData != null)
        {
            _isPooled = true;
            EnemyObjectPoolManger.Instance.ReturnObject(ProjectileData.Key, gameObject);
        }
        else
        {
            Debug.LogError("EnemyObjectPoolManger.Instance or ProjectileData is null!");
            // 풀 매니저가 없으면 객체 비활성화
            gameObject.SetActive(false);
        }
    }
}
