using UnityEngine;

public enum EnemyProjectileType
{
    Arrow,
    Stone
}
public class EnemyProjectile : MonoBehaviour, IEnemyPoolable
{
    public So_EnemyProjectile ProjectileData;

    public EnemyProjectileType type;

    private Damage _damage;
    private Vector3 _startPosision = Vector3.zero;
    private Vector3 _targetPosision = Vector3.zero;

    private const float GRAVITY = -9.81f;

    private Vector3 _velocity;
    private Vector3 _gravityVector;
    private float _timer = 0;

    private bool _isFire = false;
    private bool _isPooled = false; // Ǯ ��ȯ ���� ����

    public void OnSpawn()
    {
        _isPooled = false;
        Initialize();
    }
    public void OnDespawn()
    {
        _isPooled = true;
    }
  
    private void Awake()
    {
        if (ProjectileData != null)
        {
            _damage = new Damage(ProjectileData.Damage, this.gameObject, ProjectileData.KnockbackPower);
        }
        else
        {
            Debug.LogError($"ProjectileData is null on {gameObject.name}");
        }
    }

    public void Initialize()
    {
        _isFire = false;
        _timer = 0;
        _isPooled = false;
    }

    public void Fire(Vector3 targetPos)
    {
        if (_isPooled) return; // �̹� Ǯ�� ��ȯ�� ��ü��� �������� ����

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

        _velocity += _gravityVector * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_velocity);

        _timer += Time.deltaTime;

        if (_timer > 0.5f)
        {
            CheckGroundHit();
        }

        // �˻� ������ ä�� �ʹ� �����Ǹ� ����
        if (ProjectileData != null && _timer > ProjectileData.LostTime)
        {
            UnEnable();
        }
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
            return true; // �浹 ����
        }
        return false; // �浹 �� ��
    }
    private void OnGroundHit(RaycastHit hit)
    {
        if (_isPooled) return;

        Vector3Int blockPos = Vector3Int.FloorToInt(hit.point + hit.normal * -0.5f);

        if(type == EnemyProjectileType.Stone)
            BlockSystem.DamageBlocksInRadius(blockPos, 5f,10);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_isPooled || other == null) return;

        bool isValidTarget = other.CompareTag("Player") || other.CompareTag("Tower");

        if (other.TryGetComponent<IDamageAble>(out IDamageAble damageAble) && isValidTarget)
        {
            if (damageAble != null && _damage != null)
            {
                damageAble.TakeDamage(_damage);
            }
         

            if (type == EnemyProjectileType.Stone)
                BlockSystem.DamageBlocksInRadius(other.transform.position, 5f, 10);

            UnEnable();
        }
        else
        {
            Debug.Log("����?");
           UnEnable();
        }
    }

    private void UnEnable()
    {
        if (_isPooled) return;

        // EnemyObjectPoolManger �ν��Ͻ� Ȯ��
        if (EnemyObjectPoolManger.Instance != null && ProjectileData != null)
        {
            _isPooled = true;
            EnemyObjectPoolManger.Instance.ReturnObject(ProjectileData.Key, gameObject);
        }
        else
        {
            Debug.LogError("EnemyObjectPoolManger.Instance or ProjectileData is null!");
            // Ǯ �Ŵ����� ������ ��ü ��Ȱ��ȭ
            gameObject.SetActive(false);
        }
    }
}
