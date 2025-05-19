using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] protected GameObject HitVfxPrefab;


    [Header("설정")]
    public float gravity = -9.81f;
    public float flightTime = 1.5f; // 화살이 목표지점까지 날아가는 시간
    public float damage = 30f;

    private Vector3 _velocity;
    private Vector3 _target;
    private float _timer;
    private Vector3 _gravityVector;

    public void Init(Vector3 startPos, Vector3 targetPos, float flightDuration, float damageAmount)
    {
        _target = targetPos;
        flightTime = flightDuration;
        damage = damageAmount;

        transform.position = startPos;

        // 속도 계산
        _velocity = CalculateLaunchVelocity(startPos, targetPos, flightTime);
        _gravityVector = Vector3.up * gravity;
    }

    void Update()
    {
        // 위치 이동
        _velocity += _gravityVector * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;

        // 회전 방향 조정
        if (_velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_velocity);

        _timer += Time.deltaTime;
        if (_timer > flightTime + 0.5f) // 여유시간 지나면 제거
        {
            Destroy(gameObject);
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
    }


    private Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 end, float time)
    {
        Vector3 toTarget = end - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

        float y = toTarget.y;
        float xzDistance = toTargetXZ.magnitude;

        float vY = y / time - 0.5f * gravity * time;
        float vXZ = xzDistance / time;

        Vector3 result = toTargetXZ.normalized * vXZ;
        result.y = vY;

        return result;
    }
}
