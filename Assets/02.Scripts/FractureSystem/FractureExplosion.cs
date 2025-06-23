using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        Rigidbody[] spawnRb = _frag.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in spawnRb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.mass = 1f;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
        }
        _frag.GetComponent<FragmentShrinkAndReturn>().StartShrink();
    }
 
}
