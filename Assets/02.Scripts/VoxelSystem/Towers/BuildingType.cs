using System;
using UnityEngine;

[Serializable]
public class BuildingType
{
    public string DisplayName;
    public GameObject Prefab;
    public GameObject PreviewPrefab;
    public Material PreviewMaterial;
    public int Size = 1;
    public float YOffset = 0f;
}
