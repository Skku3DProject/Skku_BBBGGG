using UnityEngine;

public class PlayerBlockController : MonoBehaviour
{
    [Header("참조")]
    public Camera PlayerCamera;
    public LayerMask ChunkLayer;
    public LayerMask EnvirLayer;
    public float MaxDistance = 4f;

    [Header("복셀 세팅")]
    public VoxelType PlaceType = VoxelType.Grass; // 어떤 블럭을 배치할지

    void Update()
    {
        // 건설 모드에서는 동작하지 않음
        if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Build || PlayerModeManager.Instance.CurrentMode == EPlayerMode.Weapon)
            return;

        HandleBlockInput();
    }

    private void HandleBlockInput()
    {
        if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Pickaxe) // 현재 플레이어 모드를 체크하고
        {
            if (Input.GetMouseButtonDown(0)) // 좌클릭 → 블럭 파괴 or 나무나 철광석 캐기
            {
                TryDestroyBlockOrMineObject();
            }
        }
        else if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Block)// 현재 플레이어 모드를 체크하고
        {
            if (Input.GetMouseButtonDown(0)) // 우클릭 → 블럭 설치
            {
                TryPlaceBlock();
            }
        }
    }

    private void TryPlaceBlock()
    {
        Vector3Int pos = GetTargetBlockPosition(true);
        BlockSystem.PlaceBlock(pos, PlaceType);
    }

    private void TryDestroyBlockOrMineObject()
    {
        if (TryMineEnvironmentObject())
            return;

        Vector3Int pos = GetTargetBlockPosition(false);
        BlockSystem.DamageBlock(pos, 1);
    }

    private Vector3Int GetTargetBlockPosition(bool placing)
    {
        Ray ray = PlayerCamera != null
            ? PlayerCamera.ScreenPointToRay(Input.mousePosition)
            : new Ray(transform.position, transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, MaxDistance, ChunkLayer))
            return Vector3Int.zero;

        float offset = placing ? 0.5f : -0.5f;
        Vector3 adjusted = hit.point + hit.normal * offset;
        return Vector3Int.FloorToInt(adjusted);
    }

    private bool TryMineEnvironmentObject()
    {
        Ray ray = PlayerCamera != null
            ? PlayerCamera.ScreenPointToRay(Input.mousePosition)
            : new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, MaxDistance, EnvirLayer))
        {
            if (hit.collider.TryGetComponent<WorldEnvironment>(out var mineable))
            {
                mineable.TakeDamage(10); // 나중에 도구별 데미지 조정 가능
                return true;
            }
        }

        return false;
    }
}
