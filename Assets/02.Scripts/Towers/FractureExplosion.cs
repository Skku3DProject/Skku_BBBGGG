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
    [Header("Fracture Settings")]
    [SerializeField] private int fragmentCount = 10;
    [SerializeField] private Material insideMaterial;
    [SerializeField] private bool detectFloatingFragments = false;
    [SerializeField] private Vector2 textureScale = Vector2.one;
    [SerializeField] private Vector2 textureOffset = Vector2.zero;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float upwardsModifier = 0f;

    private List<GameObject> fragments = new List<GameObject>();
    private List<FragmentInfo> fragmentInfos = new List<FragmentInfo>();
    private Transform fragmentParent;

    void Awake()
    {
        GenerateFragments();
    }

    void GenerateFragments()
    {
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
            return;

        var root = new GameObject(name + "_AllFragments");
        root.transform.SetParent(transform.parent);
        root.transform.SetPositionAndRotation(transform.position, transform.rotation);
        fragmentParent = root.transform;

        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            var thisRenderer = mf.GetComponent<MeshRenderer>();
            var thisCollider = mf.GetComponent<Collider>();
            if (thisRenderer == null || thisCollider == null) continue;

            // Prefracture ����
            var localPrefracture = mf.GetComponent<Prefracture>();
            if (localPrefracture == null)
                localPrefracture = mf.gameObject.AddComponent<Prefracture>();

            localPrefracture.fractureOptions.fragmentCount = fragmentCount;
            localPrefracture.fractureOptions.insideMaterial = insideMaterial;
            localPrefracture.fractureOptions.detectFloatingFragments = detectFloatingFragments;
            localPrefracture.fractureOptions.textureScale = textureScale;
            localPrefracture.fractureOptions.textureOffset = textureOffset;
            localPrefracture.prefractureOptions.saveFragmentsToDisk = false;
            localPrefracture.prefractureOptions.saveLocation = string.Empty;

            // �����׸�Ʈ ���ø� ����
            var template = new GameObject("FragmentTemplate") { tag = mf.tag };
            template.AddComponent<MeshFilter>();
            var meshRenderer = template.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = new[]
            {
                thisRenderer.sharedMaterial,
                insideMaterial
            };

            var mc = template.AddComponent<MeshCollider>();
            mc.convex = true;
            mc.sharedMaterial = thisCollider.sharedMaterial;
            mc.isTrigger = thisCollider.isTrigger;

            var rbTemp = template.AddComponent<Rigidbody>();
            rbTemp.constraints = RigidbodyConstraints.FreezeAll;

            var rbSrc = mf.GetComponent<Rigidbody>();
            if (rbSrc)
            {
                rbTemp.linearDamping = rbSrc.linearDamping;
                rbTemp.angularDamping = rbSrc.angularDamping;
                rbTemp.useGravity = rbSrc.useGravity;
            }

            // ����ȭ ����
            Fragmenter.Fracture(
                mf.gameObject,
                localPrefracture.fractureOptions,
                template,
                fragmentParent,
                false,
                string.Empty
            );

            DestroyImmediate(template);
        }

        foreach (Transform t in fragmentParent)
        {
            var frag = t.gameObject;
            frag.SetActive(false);
            fragments.Add(frag);
            fragmentInfos.Add(new FragmentInfo
            {
                fragment = frag,
                localPosition = t.localPosition,
                localRotation = t.localRotation
            });
        }
    }

    public List<GameObject> Explode()
    {
        foreach (var info in fragmentInfos)
        {
            var frag = info.fragment;

            frag.transform.position = transform.TransformPoint(info.localPosition);
            frag.transform.rotation = transform.rotation * info.localRotation;

            frag.SetActive(true);

            var rb = frag.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.mass = 1;
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;
                rb.AddExplosionForce(
                    explosionForce,
                    transform.position,
                    explosionRadius,
                    upwardsModifier,
                    ForceMode.Impulse
                );
            }
        }

        return fragmentInfos.ConvertAll(f => f.fragment);
    }
}
