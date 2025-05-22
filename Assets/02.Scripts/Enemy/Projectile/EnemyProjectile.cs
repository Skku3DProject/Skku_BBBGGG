using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Transform Target;         // Ÿ�� (��)
    public float LaunchAngle = 45f;  // �߻� ����
    private float Gravity = -9.81f;   // �߷�
    private float FlightTime = 0f;    // ��� �ð� (������)

    private Vector3 _startPosition;
    private Vector3 _velocity;
    private bool _launched = false;

    public void Launch(Transform target, Vector3 startPosition, float angle = 45f)
    {
        Target = target;
        LaunchAngle = angle;
        _startPosition = startPosition;

        // �ʱ� �ӵ� ���
        _velocity = CalculateLaunchVelocity(Target.position, LaunchAngle);
        _launched = true;
    }

    void Update()
    {
        if (!_launched) return;

        // ������ �̵�
        _velocity.y += Gravity * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_velocity);

        FlightTime += Time.deltaTime;
        if(FlightTime > 5)
        {
            Destroy(gameObject);
        }
    }

    private Vector3 CalculateLaunchVelocity(Vector3 targetPos, float angle)
    {
        Vector3 dir = targetPos - transform.position;
        float h = dir.y;
        dir.y = 0;
        float distance = dir.magnitude;
        float rad = angle * Mathf.Deg2Rad;

        dir.y = distance * Mathf.Tan(rad);
        distance += h / Mathf.Tan(rad);

        float velocity = Mathf.Sqrt(distance * -Gravity / Mathf.Sin(2 * rad));
        return velocity * dir.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out var dmg) && other.CompareTag("Player"))
        {
            Damage _damage = new Damage(10, this.gameObject);
            dmg.TakeDamage(_damage);
            Destroy(gameObject);
        }
   
    }

}
