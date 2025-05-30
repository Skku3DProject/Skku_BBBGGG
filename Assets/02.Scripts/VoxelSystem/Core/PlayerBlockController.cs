using UnityEngine;

public class PlayerBlockController : MonoBehaviour
{
    [Header("����")]
    public Camera PlayerCamera;
    public LayerMask ChunkLayer;
    public LayerMask EnvirLayer;
    public float MaxDistance = 4f;
    private ThirdPersonPlayer _player;

    [Header("���� ����")]
    public VoxelType PlaceType = VoxelType.Stone; // � ���� ��ġ����


    //�� ��ġ ���� ���� ����
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
        // �Ǽ� ��忡���� �������� ����
        if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Build || PlayerModeManager.Instance.CurrentMode == EPlayerMode.Weapon)
            return;

        HandleBlockInput();
    }

    private void HandleBlockInput()
    {
        if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Pickaxe) // ���� �÷��̾� ��带 üũ�ϰ�
        {
            if (Input.GetMouseButton(0) && _isDig==false) // ��Ŭ�� �� �� �ı� or ������ ö���� ĳ��
            {
                _player.PlayerAnimator.SetTrigger("Attack");
                _isDig = true;
            }
        }
        else if (PlayerModeManager.Instance.CurrentMode == EPlayerMode.Block)// ���� �÷��̾� ��带 üũ�ϰ�
        {
            if (Input.GetMouseButtonDown(0)) // ��Ŭ�� �� �� ��ġ
            {
                //_player.PlayerAnimator.SetTrigger("BlockPlace"); //-- ���߿� ����ġ���

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

        // �÷��̾�� ���� ��ġ �Ÿ� üũ (��Ÿ� 100 ��� MaxDistance ���� �� ����)
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
        float distance = Vector3.Distance(playerPos, targetPos + new Vector3(0.5f, 0.5f, 0.5f)); // �� �߽��� ����
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
            // �÷��̾� ��ġ�� ��Ʈ ��ġ �� �Ÿ� ���
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

    //�ִϸ��̼� �� �νô� ���� ���� ����
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
