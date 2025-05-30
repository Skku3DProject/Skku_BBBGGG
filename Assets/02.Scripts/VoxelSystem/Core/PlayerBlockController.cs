using UnityEngine;

public class PlayerBlockController : MonoBehaviour
{
    [Header("참조")]
    public Camera PlayerCamera;
    public LayerMask ChunkLayer;
    public LayerMask EnvirLayer;
    public float MaxDistance = 4f;
    private ThirdPersonPlayer _player;

    [Header("복셀 세팅")]
    public VoxelType PlaceType = VoxelType.Stone; // 어떤 블럭을 배치할지


    //블럭 설치 연속 동작 방지
    private bool _isDig = false;

    private void Awake()
    {
        _player = GetComponent<ThirdPersonPlayer>();
    }
    private void Start()
    {
        PlayerModeManager.OnModeChanged += OnModeChanged;
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
            if (Input.GetMouseButton(0) && _isDig==false) // 좌클릭 → 블럭 파괴 or 나무나 철광석 캐기
            {
                _player.PlayerAnimator.SetTrigger("Attack");
                _isDig = true;
            }
        }
        else if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Block)// 현재 플레이어 모드를 체크하고
        {
            if (Input.GetMouseButtonDown(0)) // 우클릭 → 블럭 설치
            {
                //_player.PlayerAnimator.SetTrigger("BlockPlace"); //-- 나중에 블럭설치모션

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
    public void TryDestroyBlockOrMineObject()
    {
        if (TryMineEnvironmentObject())
            return;

        Vector3Int pos = GetTargetBlockPosition(false);

        if (!IsWithinReach(pos))
            return;

        BlockSystem.DamageBlock(pos, 10);
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

        // 플레이어와 맞은 위치 거리 체크 (사거리 100 대신 MaxDistance 쓰는 게 좋음)
        float distanceToHit = Vector3.Distance(playerPos, hit.point);
        if (distanceToHit > MaxDistance)
            return Vector3Int.zero;

        float offset = placing ? 0.5f : -0.5f;
        Vector3 adjusted = hit.point + hit.normal * offset;
        return Vector3Int.FloorToInt(adjusted);

    }
    private bool IsWithinReach(Vector3Int targetPos)
    {
        Vector3 playerPos = _player.transform.position;
        float distance = Vector3.Distance(playerPos, targetPos + new Vector3(0.5f, 0.5f, 0.5f)); // 블럭 중심점 기준
        return distance <= MaxDistance;
    }
    private bool TryMineEnvironmentObject()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f);
        Ray ray = PlayerCamera != null
            ? PlayerCamera.ScreenPointToRay(screenCenter)
            : new Ray(transform.position, transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * MaxDistance, Color.red, 30f);

        if (Physics.Raycast(ray, out RaycastHit hit, 30f, EnvirLayer))
        {
            // 플레이어 위치와 히트 위치 간 거리 계산
            float distToHit = Vector3.Distance(_player.transform.position, hit.point);
            if (distToHit > MaxDistance)
            {
                Debug.Log("Hit point is beyond MaxDistance from player.");
                return false;
            }


            if (hit.collider.TryGetComponent<WorldEnvironment>(out var mineable))
            {
                mineable.TakeDamage(10);
                return true;
            }
        }

        return false;
    }

    //애니메이션 블럭 부시는 동작 연속 방지
    public void IsDigEnd()
    {
        _isDig = false;
    }
    private void OnModeChanged(EPlayerMode type)
    {
        if (type != EPlayerMode.Pickaxe) return;

        _isDig = false;

    }

}
