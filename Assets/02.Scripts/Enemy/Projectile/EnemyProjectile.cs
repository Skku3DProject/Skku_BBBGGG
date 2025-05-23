using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Transform Target;         // Ÿ�� (��)
    public float LaunchAngle = 45f;  // �߻� ����
    private float gravity = -9.81f;   // �߷�
    private float FlightTime = 0f;    // ��� �ð� (������)

    private Vector3 _startPosition;
    private Vector3 _velocity;
    private bool _launched = false;
    private Vector3 _gravityVector;

    public void Launch(Transform target, Vector3 startPosition, float time)
    {
        Target = target;
        _startPosition = startPosition;

        // �ʱ� �ӵ� ���
        _velocity = CalculateLaunchVelocity(startPosition, target.position, time);
        _gravityVector = Vector3.up * gravity;
        _launched = true;
    }

    void Update()
    {
        if (!_launched) return;

        // ������ �̵�
        _velocity.y += gravity * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_velocity);

        FlightTime += Time.deltaTime;
        if(FlightTime > 5)
        {
            gameObject.SetActive(false);
            _launched = false;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out var dmg) && other.CompareTag("Player"))
        {
            Damage _damage = new Damage(10, this.gameObject);
            dmg.TakeDamage(_damage);
            gameObject.SetActive(false);
            _launched = false;
        }
   
    }

}
