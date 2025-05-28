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

    private bool isAiming = false;
    private bool _canShootNext = true;
    private float _comboResetTime = 1.5f;
    private float _lastAttackTime;



    [SerializeField] private BowFireSkill _fireArrow;//활 스킬
    [SerializeField] private BowThreeArrowSkill _TripleArrow;//활 스킬2

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
        Debug.Log("애니메이션 중에 쏘는거 호출됨");

        if (arrowPrefab == null || shootPoint == null || Camera.main == null) return;

        PlayerArrow arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity).GetComponent<PlayerArrow>();
        arrow.SetAttackPower(PlayerEquipmentController.Instance.GetCurrentWeaponAttackPower());

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
        if (!Input.GetMouseButtonDown(0) || !_canShootNext && _TripleArrow.IsUsingSkill == false)
        {
            Debug.Log("가운데를 향해 화살 발사 시도");
            return;
        }

        if (isAiming && _TripleArrow.IsUsingSkill == false)
        {
            ShootArrow();
            Debug.Log("가운데를 향해 화살을 쏜다");
        }


        else
        {
            //Attack();
            //Debug.Log("어택 애니메이션 실행");//일반이랑 불 화살도 이 코드 실행중

            if (Input.GetMouseButtonDown(0) && _TripleArrow.IsUsingSkill == true)
            {
                _TripleArrow.ShootThreeArrow();
                Debug.Log("화살 3개 발사 중중");
            }


            else
            {
                Attack();
                Debug.Log("어택 애니메이션 실행");//일반이랑 불 화살도 이 코드 실행중
            }
        }




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

        //조준이 완료되면 쏠 수 있도록
        if (_fireArrow != null)
        {
            _fireArrow?.Tick();
        }

        if (_TripleArrow != null)
        {
            _TripleArrow.Tick();
        }


    }

    private Vector3 CalculateShootDirection()
    {
        if (Camera.main == null)
            return shootPoint.forward;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
            return (hitInfo.point - shootPoint.position).normalized;

        return ray.direction;
    }
}