using UnityEngine;

public class BowAttack : WeaponAttackBase
{
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;

    [Header("Ȱ ����")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float shootForce = 30f;
    public TrajectoryRenderer trajectoryRenderer;
    [SerializeField] private LayerMask aimLayerMask; // Ground, Enemy �� ���ϴ� ���̾� ����

    public override bool IsAttacking { get; protected set; }

    private bool isAiming = false;
    private bool _canShootNext = true;
    private float _comboResetTime = 1.5f;
    private float _lastAttackTime;

    void Awake()
    {
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
        _player = GetComponent<ThirdPersonPlayer>();
    }

    public override void Attack()
    {
        if (!_canShootNext || IsAttacking) return;

        IsAttacking = true;
        _lastAttackTime = Time.time;
        _canShootNext = false;

        _playerAnimation.SetTrigger("Attack");
    }

    public void ShootArrow()
    {
        if (arrowPrefab == null || shootPoint == null || Camera.main == null) return;

        PlayerArrow arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity).GetComponent<PlayerArrow>();
        arrow.SetAttackPower(PlayerEquipmentController.Instance.GetCurrentWeaponAttackPower());

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb == null) return;

        // ī�޶� ȭ�� �߾ӿ��� Raycast ���
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray,out RaycastHit hitInfo, 100f))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            // �´� ���� ������ ī�޶� �� 100���� �������� ����
            targetPoint = ray.origin + ray.direction * 100f;
        }

        // �߻� ���� ��� (ȭ�� ��ġ �� ��ǥ ����)
        Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;

        // ȭ�� ȸ�� ���� (�߻� ���� ����)
        arrow.transform.rotation = Quaternion.LookRotation(shootDirection);
        arrow.transform.Rotate(90f, 0f, 0f, Space.Self);
        // ȭ�� �ӵ� ����
        rb.linearVelocity = shootDirection * shootForce;

        _canShootNext = false;
        IsAttacking = true;
        _lastAttackTime = Time.time;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 dir)
    {

    }

    public override void OnAttackEffectPlay()
    {
    }

    public override void OnAttackAnimationEnd()
    {
        _canShootNext = true;
        IsAttacking = false;
    }

    public override void EnableComboInput()
    {
        _canShootNext = true;
    }

    private void ResetAttack()
    {
        IsAttacking = false;
        _canShootNext = true;
        if (isAiming) IsAttacking = true;
    }

    public override void Tick()
    {
        HandleAimingInput();
        HandleFireInput();
        UpdateAttackCooldown();
        UpdateTrajectory();
    }

    private void HandleAimingInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetAiming(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            SetAiming(false);
            IsAttacking = false;
            _canShootNext = true;
        }
    }

    private void SetAiming(bool aiming)
    {
        isAiming = aiming;
        IsAttacking = aiming;
        if (aiming)
        {
            _player.PlayerAnimator.SetTrigger("Aim");
        }
        _player.PlayerAnimator.SetBool("IsAiming", aiming);
    }

    private void HandleFireInput()
    {
        if (!Input.GetMouseButtonDown(0) || !_canShootNext)
            return;

        if (isAiming)
            ShootArrow();
        else
            Attack();
    }

    private void UpdateAttackCooldown()
    {
        if (!IsAttacking)
            return;

        if (Time.time - _lastAttackTime > _comboResetTime)
            ResetAttack();
    }

    private void UpdateTrajectory()
    {
        if (trajectoryRenderer == null)
            return;

        if (isAiming && IsAttacking)
        {
            Vector3 initVel = CalculateShootDirection() * shootForce;
            trajectoryRenderer.DrawTrajectory(shootPoint.position, initVel);
        }
        else
        {
            trajectoryRenderer.ClearTrajectory();
        }
    }

    private Vector3 CalculateShootDirection()
    {
        if (Camera.main == null)
            return shootPoint.forward;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, aimLayerMask))
            return (hitInfo.point - shootPoint.position).normalized;

        return ray.direction;
    }
}
