using UnityEngine;

public class BowAttack : WeaponAttackBase
{
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;

    [Header("활 세팅")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float shootForce = 30f;
    public TrajectoryRenderer trajectoryRenderer;

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

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb == null) return;

        // 카메라 화면 중앙에서 Raycast 쏘기
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            // 맞는 지점 없으면 카메라 앞 100미터 방향으로 설정
            targetPoint = ray.origin + ray.direction * 100f;
        }

        // 발사 방향 계산 (화살 위치 → 목표 지점)
        Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;

        // 화살 회전 설정 (발사 방향 기준)
        arrow.transform.rotation = Quaternion.LookRotation(shootDirection);
        arrow.transform.Rotate(90f, 0f, 0f, Space.Self);
        // 화살 속도 설정
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
    }

    public override void Tick()
    {
        base.Tick();

        if (Input.GetMouseButtonDown(1)) // 오른쪽 버튼 눌렀을 때
        {
            isAiming = true;
            _player.PlayerAnimator.SetTrigger("Aim");
            _player.PlayerAnimator.SetBool("IsAiming", isAiming);
        }
        else if (Input.GetMouseButtonUp(1)) // 오른쪽 버튼 뗐을 때
        {
            isAiming = false;
            _player.PlayerAnimator.SetBool("IsAiming", isAiming);
        }

        if (Input.GetMouseButtonDown(0) && _canShootNext)
        {
            Debug.Log($"Fire input detected. isAiming={isAiming}, _canShootNext={_canShootNext}");
            if (isAiming)
            {
                ShootArrow();
            }
            else
            {
                Attack();
            }
        }

        if (IsAttacking && Time.time - _lastAttackTime > _comboResetTime)
        {
            ResetAttack();
        }

        //궤적표시
        if (trajectoryRenderer != null)
        {
            if (isAiming && !IsAttacking)
            {
                // 카메라 화면 중앙에서 Raycast 쏘기
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                Vector3 targetPoint;

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
                {
                    targetPoint = hitInfo.point;
                }
                else
                {
                    targetPoint = ray.origin + ray.direction * 100f;
                }

                Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;
                Vector3 initVel = shootDirection * shootForce;

                trajectoryRenderer.DrawTrajectory(shootPoint.position, initVel);
            }
            else
            {
                trajectoryRenderer.ClearTrajectory();
            }
        }
    }
}
