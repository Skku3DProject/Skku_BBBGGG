using UnityEngine;

public class PlayerBlockController : MonoBehaviour
{
    [Header("����")]
    public Camera PlayerCamera;
    public LayerMask ChunkLayer;
    public LayerMask EnvirLayer;
    public float MaxDistance = 4f;

    [Header("���� ����")]
    public VoxelType PlaceType = VoxelType.Grass; // � ���� ��ġ����

    void Update()
    {
        // �Ǽ� ��忡���� �������� ����
        if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Build || PlayerModeManager.Instance.CurrentMode == EPlayerMode.Weapon)
            return;

        HandleBlockInput();
    }

    private void HandleBlockInput()
    {
        if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Pickaxe) // ���� �÷��̾� ��带 üũ�ϰ�
        {
            if (Input.GetMouseButtonDown(0)) // ��Ŭ�� �� �� �ı� or ������ ö���� ĳ��
            {
                TryDestroyBlockOrMineObject();
            }
        }
        else if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Block)// ���� �÷��̾� ��带 üũ�ϰ�
        {
            if (Input.GetMouseButtonDown(0)) // ��Ŭ�� �� �� ��ġ
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
                mineable.TakeDamage(10); // ���߿� ������ ������ ���� ����
                return true;
            }
        }

        return false;
    }
}
