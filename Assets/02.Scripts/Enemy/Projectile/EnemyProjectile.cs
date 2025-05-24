using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public So_EnemyProjectile ProjectileData;

    private Damage _damage;
    private Transform _parentTranfrom;
    private Vector3 _startPosision = Vector3.zero;
    private Vector3 _targetPosision = Vector3.zero;

    private const float GRAVITY = -9.81f;

    private Vector3 _velocity;
    private Vector3 _gravityVector;
    private float _timer = 0;

    private bool _isFire = false;
    private bool _checkedGroundHit = false;

    private void Awake()
    {
        _damage = new Damage(ProjectileData.Damage, this.gameObject, ProjectileData.KnockbackPower);
    }

    public void SetParentTranfrom(Transform transform)
    {
        _parentTranfrom = transform;
    }

    public void Initialize()
    {
        transform.SetParent(_parentTranfrom);
        _isFire = false;
        _timer = 0;
    }

    public void Fire(Vector3 startPos, Vector3 targetPos)
    {
        gameObject.SetActive(true);
        transform.SetParent(null);

        _startPosision = startPos;
        _targetPosision = targetPos;
        transform.position = startPos;

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

        if (_timer > ProjectileData.FlightTime && _timer <= ProjectileData.FlightTime + 5f && !_checkedGroundHit)
        {
            if (CheckGroundHit()) // true면 충돌 성공
            {
                _checkedGroundHit = true;
            }
        }

        // 검사 실패한 채로 너무 오래되면 삭제
        if (_timer > ProjectileData.LostTime)
        {
            UnEnable();
        }
    }
    private Vector3 CalculateLaunchVelocity()
    {
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
    protected virtual bool CheckGroundHit()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, ProjectileData.GroundMask))
        {
            OnGroundHit(hit);
            return true; // 충돌 성공
        }
        return false; // 충돌 못 함
    }
    private void OnGroundHit(RaycastHit hit)
    {
        Vector3Int blockPos = Vector3Int.FloorToInt(hit.point + hit.normal * -0.5f);
        // BlockSystem.DamageBlock(blockPos, 10);
        UnEnable();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out IDamageAble damageAble) && other.CompareTag("Player"))
        {
            damageAble.TakeDamage(_damage);
            UnEnable();
        }
    }

    private void UnEnable()
    {
        gameObject.SetActive(false);
        transform.SetParent(_parentTranfrom);
    }
}
