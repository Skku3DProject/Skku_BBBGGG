using UnityEngine;


public class EnemyProjectile : MonoBehaviour, IEnemyPoolable
{
    public So_EnemyProjectile ProjectileData;

    private TrailRenderer _trailRenderer;
    private Damage _damage;
    private Vector3 _startPosision = Vector3.zero;
    private Vector3 _targetPosision = Vector3.zero;
    //private EnemySoundManager _soundManager;

    private const float GRAVITY = -9.81f;

    private Vector3 _velocity;
    private Vector3 _gravityVector;
    private float _timer = 0;

    private bool _isFire = false;
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

            //_soundManager = GetComponent<EnemySoundManager>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
        }
        else
        {
            Debug.LogError($"ProjectileData is null on {gameObject.name}");
        }
    }
    public void Initialize()
    {
        if (_trailRenderer != null)
        {
            _trailRenderer.Clear();
            _trailRenderer.enabled = false;
        }
        _isFire = false;
        _timer = 0;
        _isPooled = false;
        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
        }
    }

    public void Fire(Vector3 targetPos)
    {
        if (_isPooled)
        {
            UnEnable();
            return; // 이미 풀에 반환된 객체라면 실행하지 않음
        }
        //   gameObject.SetActive(true);
        if (_trailRenderer != null)
        {
            _trailRenderer.enabled = true;
        }
        _startPosision = transform.position;
        _targetPosision = targetPos;

        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
        }

        _velocity = CalculateLaunchVelocity();
        _gravityVector = Vector3.up * GRAVITY;

        _isFire = true;
    }

    private void Update()
    {
        if (!_isFire) return;

        _timer += Time.deltaTime;
        if (ProjectileData != null && _timer > ProjectileData.LostTime)
        {
            UnEnable();
        }

        _velocity += _gravityVector * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_velocity);

        if (_timer > 0.5f && ProjectileData.Type == EnemyProjectileType.Area)
        {
            CheckGroundHit();
        }

        // 검사 실패한 채로 너무 오래되면 삭제

    }
    private Vector3 CalculateLaunchVelocity()
    {
        if (ProjectileData == null)
        {
            Debug.LogError("ProjectileData is null in CalculateLaunchVelocity");
            return Vector3.zero;
        }

        Vector3 toTarget = _targetPosision - _startPosision;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

        float y = toTarget.y;
        float xzDistance = toTargetXZ.magnitude;

        float vY = y / ProjectileData.FlightTime - 0.5f * GRAVITY * ProjectileData.FlightTime;
        float vXZ = xzDistance / ProjectileData.FlightTime;

        Vector3 result = toTargetXZ.normalized * vXZ;
        result.y = vY;

        return result;
    }
    private bool CheckGroundHit()
    {
        if (_isPooled || ProjectileData == null) return false;

        Vector3 forward = transform.forward;
        Vector3 down = Vector3.down;
        Vector3 midDirection = (forward + down).normalized;

        if (Physics.Raycast(transform.position, midDirection, out RaycastHit hit, 0.5f, ProjectileData.GroundMask))
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
        BlockManager.Instance.DamageBlocksInRadius(blockPos, ProjectileData.AreaRange, (int)_damage.Value);
        EnemyParticlePoolManger.Instance.GetObject(ProjectileData.HitVfxKey, blockPos);
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
        EnemyParticlePoolManger.Instance.GetObject(ProjectileData.HitVfxKey, transform.position);
        //_soundManager.PlaySound("Effect");
    }

    private void AreaAttack(Collider other)
    {
        BlockManager.Instance.DamageBlocksInRadius(transform.position, ProjectileData.AreaRange, (int)_damage.Value);

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

        UnEnable();
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
        // EnemyObjectPoolManger 인스턴스 확인
        if (EnemyObjectPoolManger.Instance != null && ProjectileData != null)
        {
            _isPooled = true;
            EnemyObjectPoolManger.Instance.ReturnObject(ProjectileData.Key, gameObject);
            //_soundManager.PlaySound("Effect");
        }
        else
        {
            Debug.LogError("EnemyObjectPoolManger.Instance or ProjectileData is null!");
            // 풀 매니저가 없으면 객체 비활성화
            gameObject.SetActive(false);
        }
    }
}
