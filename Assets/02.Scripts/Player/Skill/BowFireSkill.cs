using UnityEngine;
using System.Collections;

public class BowFireSkill : WeaponSkillBase
{
    public GameObject FireEffect;

    public GameObject MyPlayer;
    private Animator _playerAnimator;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;
    private PlayerAttack _playerAttack;
    [Header("Skill Settings")]
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float _skillDamageMultiplier = 0.55f;
    [SerializeField] private float _arrowForce = 100f;
    [SerializeField] private float _arrowLifetime = 5f;
    [SerializeField] private float _skillDuration = 5f;

    [Header("Arrow Spawn Point")]
    [SerializeField] private Transform _bowArrowSpawnPoint;
    [SerializeField] private Vector3 _arrowModelRotationOffset = new Vector3(90f, 0f, 0f);

    public bool CurrentArrowFireSkill;
    public override bool IsUsingSkill { get; protected set; }
    private bool _isAttacking;

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimator = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _playerAttack = GetComponent<PlayerAttack>();
    }

    public override void UseSkill()
    {
        if (IsCooldown) return;


        FireEffect.SetActive(true);

        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Bow)
        {
            return;
        }

        IsUsingSkill = true;
        CurrentArrowFireSkill= true;
        _player.CharacterController.stepOffset = 0f;
        StartCoroutine(EndSkillAfterDelay(_skillDuration));
    }

    private IEnumerator EndSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;
        CurrentArrowFireSkill = false;
        FireEffect.SetActive(false);

        //쿨타임 초기화
        cooldownTimer = cooldownTime;
    }

    public void ShootFireArrow()
    {
        if( CurrentArrowFireSkill && !_isAttacking)
        {
            _playerAnimator.SetTrigger("FireArrowAttack");
            _playerAttack.IsUsingJumpAnim = false;
            _playerAttack.IsMoveSlow = true;
            _isAttacking = true;
            
        }
    }

    public void FireSingleArrow()
    {
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

        Vector3 shootDirection = (targetPoint - _bowArrowSpawnPoint.position).normalized;

        float power = _playerAttack.CurrentDamage * _skillDamageMultiplier;

        PlayerSoundController.Instance.PlaySound(PlayerSoundType.BowSkill1);


        PlayerArrow arrowInstance = Instantiate(_arrowPrefab, _bowArrowSpawnPoint.position, Quaternion.LookRotation(shootDirection)).GetComponent<PlayerArrow>();
        arrowInstance.ArrowInit(power,ArrowType.Explosive,_player.gameObject);
        arrowInstance.transform.Rotate(_arrowModelRotationOffset, Space.Self);

        Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            StartCoroutine(ApplyForceToArrowAfterDelay(rb, shootDirection.normalized * _arrowForce, _arrowLifetime));
        }
    }

    private IEnumerator ApplyForceToArrowAfterDelay(Rigidbody rb, Vector3 force, float lifetime)
    {
        yield return new WaitForFixedUpdate();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        _isAttacking = false;
        _playerAttack.IsUsingJumpAnim = true;
        _playerAttack.IsMoveSlow = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void OnSkillAnimationEnd()
    { 
    }

    public override void Tick()
    {
        base.Tick();
        UIManager.instance.UI_CooltimeRefresh(ESkillButton.BowQ, CooldownRemaining);
        if (!CurrentArrowFireSkill) return;

        // 이건 스킬 안에서만 처리하는 Mouse0 입력
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!_isAttacking)
            {
                Debug.Log("FireARROW");

                ShootFireArrow();
            }
        }

    }

    public override void OnSkillEffectPlay() { }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
    }

    public override void ResetState()
    {
        // 상태 플래그 초기화
        IsUsingSkill = false;
        CurrentArrowFireSkill = false;
        _isAttacking = false;

        // 캐릭터 이동 특성 복구
        if (_player != null && _player.CharacterController != null)
            _player.CharacterController.stepOffset = 1f;

        // 이펙트 비활성화
        if (FireEffect != null)
            FireEffect.SetActive(false);

        // 점프 애니메이션 & 이동 속도 제한 복원
        if (_playerAttack != null)
        {
            _playerAttack.IsUsingJumpAnim = true;
            _playerAttack.IsMoveSlow = false;
        }

        // 애니메이션 상태 초기화
        if (_playerAnimator != null)
        {
            _playerAnimator.ResetTrigger("FireArrowAttack");
            _playerAnimator.SetTrigger("Idle");
        }

        //// 코루틴이 있다면 StopAllCoroutines() 고려 (불필요한 force 적용 방지)
        //StopAllCoroutines();

        Debug.Log("BowFireSkill 상태 초기화 완료");
    }
}
