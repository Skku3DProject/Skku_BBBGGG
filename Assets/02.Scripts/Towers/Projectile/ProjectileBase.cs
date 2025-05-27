using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ProjectileBase : MonoBehaviour
{
    [Header("����")]
    [SerializeField] protected GameObject HitVfxPrefab;
    [SerializeField] private LayerMask _groundMask;

    [Header("����")]
    public float gravity = -9.81f;
    public float flightTime = 1.5f; // ȭ���� ��ǥ�������� ���ư��� �ð�

    protected SO_TowerData _data;
    private Vector3 _target;
    private Vector3 _velocity;
    private float _timer;
    private Vector3 _gravityVector;
    protected GameObject _owner;

    private void OnEnable()
    {
        _timer = 0f;
    }

    public void Init(Vector3 startPos, Vector3 targetPos, float flightDuration, GameObject owner,SO_TowerData data = null)
    {
        _target = targetPos;
        flightTime = flightDuration;
        _data = data;

        transform.position = startPos;
        _owner = owner;
        // �ӵ� ���
        _velocity = CalculateLaunchVelocity(startPos, targetPos, flightTime);
        _gravityVector = Vector3.up * gravity;
    }

    void Update()
    {
        _velocity += _gravityVector * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;

        if (_velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_velocity);

        _timer += Time.deltaTime;

        // flightTime ���ĺ��� ���� �ð� ���� ��� �浹 �˻� �õ�
        if (_timer > flightTime && _timer <= flightTime + 5f && !_checkedGroundHit)
        {
            if (CheckGroundHit()) // true�� �浹 ����
            {
                _checkedGroundHit = true;
                ObjectPool.Instance.ReturnToPool(gameObject);
                //Destroy(gameObject);
            }
        }

        // �˻� ������ ä�� �ʹ� �����Ǹ� ����
        if (_timer > flightTime + 10f)
        {
            ObjectPool.Instance.ReturnToPool(gameObject);
            //Destroy(gameObject);
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {

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

    private bool _checkedGroundHit = false;

    protected virtual bool CheckGroundHit()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, _groundMask))
        {
            Debug.Log("Raycast Hit Ground: " + hit.collider.name);

            OnGroundHit(hit);
            //if (HitVfxPrefab)
            //{
            //    Instantiate(HitVfxPrefab, hit.point, Quaternion.identity);
            //}

            return true; // �浹 ����
        }

        return false; // �浹 �� ��
    }
    protected virtual void OnGroundHit(RaycastHit hit)
    {
        if (HitVfxPrefab)
        {
            ObjectPool.Instance.GetObject(HitVfxPrefab,hit.point, Quaternion.identity);
            //Instantiate(HitVfxPrefab, hit.point, Quaternion.identity);
        }
    }
}
