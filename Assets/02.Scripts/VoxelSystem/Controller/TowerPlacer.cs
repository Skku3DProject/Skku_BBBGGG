using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    [Header("건물 타입 목록")]
    public List<BuildingType> BuildingTypes;
    public int SelectedTypeIndex = 0;

    [Header("설치 관련")]
    public LayerMask GroundMask;
    public LayerMask ObstacleMask;

    private readonly Color _canPlaceColor = new Color(0f, 0.5f, 1f, 0.4f);
    private readonly Color _cannotPlaceColor = new Color(1f, 0f, 0f, 0.4f);

    private GameObject _previewInstance;
    private bool _canPlace = false;
    private Quaternion _currentRotation = Quaternion.identity;

    public BuildingType CurrentType => BuildingTypes[SelectedTypeIndex];

    void Update()
    {
        TrySetSelectedBuildingType();
        TryToggleBuildMode();
        TryRotatePreview();

        if (!BuildModeManager.Instance.IsBuildMode || _previewInstance == null)
            return;

        UpdatePreviewPosition();
        HandleBuildInput();
    }

    void TrySetSelectedBuildingType()
    {
        for (int i = 0; i < BuildingTypes.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectedTypeIndex = i;
                RefreshPreviewInstance();
            }
        }
    }

    void TryToggleBuildMode()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            BuildModeManager.Instance.ToggleBuildMode();
            if (BuildModeManager.Instance.IsBuildMode)
                RefreshPreviewInstance();
            else
                DestroyPreview();
        }
    }

    void TryRotatePreview()
    {
        if (BuildModeManager.Instance.IsBuildMode && _previewInstance != null && Input.GetKeyDown(KeyCode.R))
        {
            _currentRotation *= Quaternion.Euler(0f, 90f, 0f);
            _previewInstance.transform.rotation = _currentRotation;
        }
    }

    void RefreshPreviewInstance()
    {
        DestroyPreview();
        _previewInstance = Instantiate(CurrentType.PreviewPrefab);
        _currentRotation = Quaternion.identity;
        _previewInstance.transform.rotation = _currentRotation;
    }

    void DestroyPreview()
    {
        if (_previewInstance != null)
            Destroy(_previewInstance);
        _previewInstance = null;
    }

    void UpdatePreviewPosition()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, GroundMask))
            return;

        int size = CurrentType.Size;
        int gx = Mathf.FloorToInt(hit.point.x);
        int gz = Mathf.FloorToInt(hit.point.z);
        int startX = gx - size / 2;
        int startZ = gz - size / 2;

        if (!HasFlatGround(startX, startZ, size, hit.point.y, out float referenceY))
        {
            _canPlace = false;
            SetPreviewColor(_previewInstance, _cannotPlaceColor, CurrentType.PreviewMaterial);
            return;
        }

        Vector3 finalPos = CalculateFinalPosition(startX, startZ, size, referenceY);
        _previewInstance.transform.position = finalPos;

        Collider[] cols = Physics.OverlapBox(
            finalPos,
            new Vector3(size / 2f, GetPreviewHeightOffset(), size / 2f),
            Quaternion.identity,
            ObstacleMask
        );

        _canPlace = cols.Length == 0;
        SetPreviewColor(_previewInstance, _canPlace ? _canPlaceColor : _cannotPlaceColor, CurrentType.PreviewMaterial);
    }

    Vector3 CalculateFinalPosition(int startX, int startZ, int size, float baseY)
    {
        float x = startX + size / 2f;
        float z = startZ + size / 2f;
        float yOffset = GetPreviewHeightOffset();
        return new Vector3(x, baseY + yOffset + CurrentType.YOffset, z);
    }

    float GetPreviewHeightOffset()
    {
        return CurrentType.PreviewPrefab.GetComponentInChildren<Renderer>().bounds.extents.y;
    }

    bool HasFlatGround(int startX, int startZ, int size, float rayY, out float referenceY)
    {
        referenceY = float.NaN;

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                Vector3 origin = new Vector3(startX + x + 0.5f, rayY + 1f, startZ + z + 0.5f);
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 1.1f, GroundMask))
                {
                    if (float.IsNaN(referenceY))
                        referenceY = hit.point.y;
                    else if (Mathf.Abs(hit.point.y - referenceY) > 0.01f)
                        return false;
                }
                else return false;
            }
        }

        return true;
    }

    void HandleBuildInput()
    {
        if (Input.GetMouseButtonDown(0) && _canPlace)
        {
            //Instantiate(CurrentType.Prefab, _previewInstance.transform.position, _previewInstance.transform.rotation);
            ObjectPool.Instance.GetObject(CurrentType.Prefab, _previewInstance.transform.position, _previewInstance.transform.rotation);
            BuildModeManager.Instance.SetBuildMode(false);
            DestroyPreview();
        }
    }

    void SetPreviewColor(GameObject obj, Color color, Material template)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            Material mat = new Material(template);
            mat.color = color;
            r.material = mat;
        }
    }
}