using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance { get; private set; }

    //[Header("건물 목록")]
    //public List<BuildingType> BuildingTypes;
    public GameObject TowerPlaceVfxPrefab;

    [Header("레이어 설정")]
    public LayerMask GroundMask;
    public LayerMask ObstacleMask;

    private readonly Color _canPlaceColor = new Color(0f, 0.5f, 1f, 0.4f);
    private readonly Color _cannotPlaceColor = new Color(1f, 0f, 0f, 0.4f);

    private GameObject _previewInstance;
    private BuildingType _selectedBuilding;
    private bool _canPlace = false;
    private Quaternion _currentRotation = Quaternion.identity;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerModeManager.OnModeChanged += OnModeChanged;
    }

    private void OnDisable()
    {
        PlayerModeManager.OnModeChanged -= OnModeChanged;
    }

    private void OnModeChanged(EPlayerMode mode)
    {
        if (mode != EPlayerMode.Build)
        {
            DestroyPreview();
            _selectedBuilding = null;
        }
    }

    public void SetSelectedBuilding(BuildingType building)
    {
        _selectedBuilding = building;
        RefreshPreviewInstance();
    }

    private void Update()
    {
        if (PlayerModeManager.Instance.CurrentMode != EPlayerMode.Build || _selectedBuilding == null)
        {
            DestroyPreview();
            return;
        }

        TryRotatePreview();
        if (_previewInstance == null)
            RefreshPreviewInstance();

        UpdatePreviewPosition();
        HandleBuildInput();
    }

    private void TryRotatePreview()
    {
        if (Input.GetKeyDown(KeyCode.T) && _previewInstance != null)
        {
            _currentRotation *= Quaternion.Euler(0f, 90f, 0f);
            _previewInstance.transform.rotation = _currentRotation;
        }
    }

    private void RefreshPreviewInstance()
    {
        DestroyPreview();
        _previewInstance = Instantiate(_selectedBuilding.PreviewPrefab);
        _currentRotation = Quaternion.identity;
        _previewInstance.transform.rotation = _currentRotation;
    }

    private void DestroyPreview()
    {
        if (_previewInstance != null)
            Destroy(_previewInstance);
        _previewInstance = null;
    }

    private void UpdatePreviewPosition()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, GroundMask))
            return;

        int size = _selectedBuilding.Size;
        int gx = Mathf.FloorToInt(hit.point.x);
        int gz = Mathf.FloorToInt(hit.point.z);
        int startX = gx - size / 2;
        int startZ = gz - size / 2;

        if (!HasFlatGround(startX, startZ, size, hit.point.y, out float referenceY))
        {
            _canPlace = false;
            SetPreviewColor(_previewInstance, _cannotPlaceColor, _selectedBuilding.PreviewMaterial);
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
        SetPreviewColor(_previewInstance, _canPlace ? _canPlaceColor : _cannotPlaceColor, _selectedBuilding.PreviewMaterial);
    }

    private Vector3 CalculateFinalPosition(int startX, int startZ, int size, float baseY)
    {
        float x = startX + size / 2f;
        float z = startZ + size / 2f;
        float yOffset = GetPreviewHeightOffset();
        return new Vector3(x, baseY + yOffset + _selectedBuilding.YOffset, z);
    }

    private float GetPreviewHeightOffset()
    {
        return _selectedBuilding.PreviewPrefab.GetComponentInChildren<Renderer>().bounds.extents.y;
    }

    private bool HasFlatGround(int startX, int startZ, int size, float rayY, out float referenceY)
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

    private void HandleBuildInput()
    {
        if (Input.GetMouseButtonDown(0) && _canPlace)
        {
            if (!CurrencyManager.instance.Spend(_selectedBuilding.cost))
            {
                PlayerModeManager.Instance.SetMode(EPlayerMode.Weapon); // 건설 후 무기 모드 복귀
                return;
            }

            TowerSoundController.Instance.PlaySoundAt(TowerSoundType.Collapse, _previewInstance.transform.position); // 건설소리
            ObjectPool.Instance.GetObject(TowerPlaceVfxPrefab, _previewInstance.transform.position, _previewInstance.transform.rotation);
            ObjectPool.Instance.GetObject(_selectedBuilding.Prefab, _previewInstance.transform.position, _previewInstance.transform.rotation);
            PlayerModeManager.Instance.SetMode(EPlayerMode.Weapon); // 건설 후 무기 모드 복귀
            DestroyPreview();
            _selectedBuilding = null;

            UI_TowerBuildMenu.isBuildMode = false;

        }
    }

    private void SetPreviewColor(GameObject obj, Color color, Material template)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            Material mat = new Material(template);
            mat.color = color;
            r.material = mat;
        }
    }
}