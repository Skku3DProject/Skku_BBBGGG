using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerBlockController : MonoBehaviour
{
    [Header("����")]
    public Camera PlayerCamera;
    public LayerMask ChunkLayer;
    public LayerMask EnvirLayer;
    public float MaxDistance = 4f;
    private Player _player;

    [Header("���� ����")]
    public VoxelType PlaceType = VoxelType.Grass; // � ���� ��ġ����

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

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
                _player.Animator.SetTrigger("PickaxeDig");
            }
        }
        else if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Block)// ���� �÷��̾� ��带 üũ�ϰ�
        {
            if (Input.GetMouseButtonDown(0)) // ��Ŭ�� �� �� ��ġ
            {
                //_player.Animator.SetTrigger("BlockPlace"); -- ���߿� ����ġ���

                TryPlaceBlock();
            }
        }
    }

    //�ִ� �̺�Ʈ��
    private void TryPlaceBlock()
    {
        Vector3Int pos = GetTargetBlockPosition(true);
        if (!IsWithinReach(pos))
            return;
        BlockSystem.PlaceBlock(pos, PlaceType);
    }
    //�ִ� �̺�Ʈ��
    private void TryDestroyBlockOrMineObject()
    {
        if (TryMineEnvironmentObject())
            return;

        Vector3Int pos = GetTargetBlockPosition(false);

        if (!IsWithinReach(pos))
            return;

        BlockSystem.DamageBlock(pos, 1);
    }

    private Vector3Int GetTargetBlockPosition(bool placing)
    {
        Vector3 playerPos = _player.transform.position;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f);
        Ray ray = PlayerCamera != null
            ? PlayerCamera.ScreenPointToRay(screenCenter)
            : new Ray(transform.position, transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100, ChunkLayer))
            return Vector3Int.zero;

        float offset = placing ? 0.5f : -0.5f;
        Vector3 adjusted = hit.point + hit.normal * offset;
        return Vector3Int.FloorToInt(adjusted);

        //Ray ray = PlayerCamera != null
        //    ? PlayerCamera.ScreenPointToRay(Input.mousePosition)
        //    : new Ray(transform.position, transform.forward);

        //if (!Physics.Raycast(ray, out RaycastHit hit, MaxDistance, ChunkLayer))
        //    return Vector3Int.zero;

        //float offset = placing ? 0.5f : -0.5f;
        //Vector3 adjusted = hit.point + hit.normal * offset;
        //return Vector3Int.FloorToInt(adjusted);
    }
    private bool IsWithinReach(Vector3Int targetPos)
    {
        Vector3 playerPos = _player.transform.position;
        float distance = Vector3.Distance(playerPos, targetPos + new Vector3(0.5f, 0.5f, 0.5f)); // �� �߽��� ����
        return distance <= MaxDistance;
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
