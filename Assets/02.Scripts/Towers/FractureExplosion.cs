using System.Collections.Generic;
using UnityEngine;
public class FragmentInfo
{
    public GameObject fragment;
    public Vector3 localPosition;
    public Quaternion localRotation;
}
public class FractureExplosion : MonoBehaviour
{
    public GameObject fragmentPrefabs; // 폭발에 사용할 프래그먼트 프리팹 리스트
    GameObject _frag;

    [Header("Explosion Settings")]
    public float explosionForce = 500f;
    public float explosionRadius = 5f;
    public float upwardsModifier = 0f;

    public void Explode()
    {
        _frag = ObjectPool.Instance.GetObject(fragmentPrefabs, transform.position, transform.rotation);

        Rigidbody[] spawnedFragments = _frag.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in spawnedFragments)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.mass = 1f;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
        }

    }

    public void ReturnFragmentsToPool()
    {
        // 위치 초기화 (필요에 따라 원하는 기본 위치로)
        _frag.transform.position = Vector3.zero;
        _frag.transform.rotation = Quaternion.identity;

        // 자식들 위치도 초기화
        foreach (Transform child in _frag.transform)
        {
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
        }
        ObjectPool.Instance.ReturnToPool(_frag);
    }
}
