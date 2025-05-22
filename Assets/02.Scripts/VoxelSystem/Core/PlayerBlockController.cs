using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerBlockController : MonoBehaviour
{
    [Header("참조")]
    public Camera PlayerCamera;
    public LayerMask ChunkLayer;
    public LayerMask EnvirLayer;
    public float MaxDistance = 4f;
    private Player _player;

    [Header("복셀 세팅")]
    public VoxelType PlaceType = VoxelType.Grass; // 어떤 블럭을 배치할지

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

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
                _player.Animator.SetTrigger("PickaxeDig");
            }
        }
        else if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Block)// 현재 플레이어 모드를 체크하고
        {
            if (Input.GetMouseButtonDown(0)) // 우클릭 → 블럭 설치
            {
                //_player.Animator.SetTrigger("BlockPlace"); -- 나중에 블럭설치모션

                TryPlaceBlock();
            }
        }
    }

    //애님 이벤트용
    private void TryPlaceBlock()
    {
        Vector3Int pos = GetTargetBlockPosition(true);
        if (!IsWithinReach(pos))
            return;
        BlockSystem.PlaceBlock(pos, PlaceType);
    }
    //애님 이벤트용
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
        float distance = Vector3.Distance(playerPos, targetPos + new Vector3(0.5f, 0.5f, 0.5f)); // 블럭 중심점 기준
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
                mineable.TakeDamage(10); // 나중에 도구별 데미지 조정 가능
                return true;
            }
        }

        return false;
    }
}
