using UnityEngine;

public class PlayerBlockController : MonoBehaviour
{
    [Header("����")]
    public Camera PlayerCamera;
    public LayerMask ChunkLayer;
    public float MaxDistance = 4f;

    [Header("���� ����")]
    public VoxelType PlaceType = VoxelType.Grass;

    void Update()
    {
        // �Ǽ� ��忡���� �������� ����
        if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Build || PlayerModeManager.Instance.CurrentMode == EPlayerMode.Weapon)
            return;

        HandleBlockInput();
    }

    private void HandleBlockInput()
    {
        if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Pickaxe)
        {
            if (Input.GetMouseButtonDown(0)) // ��Ŭ�� �� �� �ı�
            {
                Vector3Int pos = GetTargetBlockPosition(false);
                BlockSystem.DamageBlock(pos, 1);
            }
        }
        else if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Block)
        {
            if (Input.GetMouseButtonDown(0)) // ��Ŭ�� �� �� ��ġ
            {
                Vector3Int pos = GetTargetBlockPosition(true);
                BlockSystem.PlaceBlock(pos, PlaceType);
            }
        }
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
}
