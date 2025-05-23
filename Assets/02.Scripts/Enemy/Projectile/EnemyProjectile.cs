using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _endPos = Vector3.zero;
    public float speed = 2;

    private float _timar = 0;

    private bool _isFire = false;
    public void Init(Vector3 startPos , Vector3 endPos)
    {
        _startPos = startPos;
        _endPos = endPos;

        transform.position = _startPos;
    }

    public void Fire()
    {
        _timar = 0;
        _isFire = true;
    }

    private void Update()
    {
        if (!_isFire) return;

        Vector3 targetPos = (_endPos - _startPos).normalized;

        transform.position = transform.position + targetPos * speed * Time.deltaTime;

        _timar += Time.deltaTime;
        if (2 < _timar)
        {
            Unenalbe();
        }
    }

    private void OnGroundHit(RaycastHit hit)
    {
        Vector3Int blockPos = Vector3Int.FloorToInt(hit.point + hit.normal * -0.5f);
        BlockSystem.DamageBlock(blockPos, 10);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out var d) && other.CompareTag("Player"))
        {
            d.TakeDamage(new Damage(10, gameObject));
        }

        /*
        if (HitVfxPrefab)
        {
            ObjectPool.Instance.GetObject(HitVfxPrefab, other.transform.position, other.transform.rotation);
        }
        */
        Unenalbe();
    }

    private void Unenalbe()
    {
        gameObject.SetActive(false);
        _timar = 0;
        _isFire = false;
    }
}
