using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class SwordSpinSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;
    [SerializeField] private float _skillDamageMultiplier = 2.5f;

    private SwordDashSkill _swordDashSkill;
    public bool CurrentSwordSpinSkill;
    public override bool IsUsingSkill { get; protected set; }
    public bool IsAttacking;

    private Coroutine _spinCoroutine; // 중복 방지용 코루틴 핸들

    private float _baseAnimationDuration = 0.2f;  // 애니메이션 원래 길이
    private float _animationSpeed = 1.0f;         // 애니메이션 재생 속도
    private int _repeatCount = 15;                  // 반복 재생 횟수

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = MyPlayer.GetComponent<Animator>();
        _equipmentController = MyPlayer.GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _swordDashSkill = MyPlayer.GetComponent<SwordDashSkill>();

        // 애니메이션 속도 설정
        _playerAnimation.SetFloat("SpinAttackSpeed", _animationSpeed);
    }

    public override void UseSkill()
    {
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Sword)
            return;

        if (!IsAttacking && !IsUsingSkill && !CurrentSwordSpinSkill)
        {
            IsAttacking = true;
            IsUsingSkill = true;
            CurrentSwordSpinSkill = true;

            _player.CharacterController.stepOffset = 0f;

            if (_spinCoroutine != null)
                StopCoroutine(_spinCoroutine);

            _spinCoroutine = StartCoroutine(PlaySpinAnimationMultipleTimes(_repeatCount, _baseAnimationDuration, _animationSpeed));

            Debug.Log("스핀 시작");

            _swordDashSkill.ShieldCollider.enabled = false;
        }
    }

    private IEnumerator PlaySpinAnimationMultipleTimes(int repeatCount, float baseDuration, float speedMultiplier)
    {
        float actualDuration = baseDuration / speedMultiplier;

        for (int i = 0; i < repeatCount; i++)
        {
            _playerAnimation.ResetTrigger("SpinAttack");
            _playerAnimation.SetTrigger("SpinAttack");
            yield return new WaitForSeconds(actualDuration);
        }

        _playerAnimation.SetTrigger("Idle");
        _player.CharacterController.stepOffset = 1f;

        IsUsingSkill = false;
        IsAttacking = false;
        CurrentSwordSpinSkill = false;

        Debug.Log("스핀 종료");
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
            _swordDashSkill.CurrentSwordDashSkill = false;
            // CurrentSwordSpinSkill = true; // UseSkill 내에서 처리하므로 중복 제거 가능
        }
    }

    public override void OnSkillEffectPlay()
    {
        // 필요시 확장 가능
    }

    public override void OnSkillAnimationEnd()
    {
        IsAttacking = false;
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if (!IsUsingSkill)
            return;

        float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier;
        IDamageAble damageAble = enemy.GetComponent<IDamageAble>();
        if (damageAble != null)
        {
            Damage damage = new Damage(power, gameObject, 100f, hitDirection);
            damageAble.TakeDamage(damage);
            Debug.Log($"회전베기 스킬로 {enemy.name}에게 {power}데미지를 입혔다!");
        }
    }
}

/*using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class SwordSpinSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;
    [SerializeField] private float _skillDamageMultiplier = 2.5f;

    private SwordDashSkill _swordDashSkill;
    public bool CurrentSwordSpinSkill;
    public override bool IsUsingSkill { get; protected set; }
    public bool IsAttacking;

    private Coroutine _spinCoroutine; // 중복 방지용 코루틴 핸들

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = MyPlayer.GetComponent<Animator>();
        _equipmentController = MyPlayer.GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _swordDashSkill = MyPlayer.GetComponent<SwordDashSkill>();
    }

    public override void UseSkill()
    {
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Sword)
            return;

        if (!IsAttacking && !IsUsingSkill && !CurrentSwordSpinSkill)
        {
            _playerAnimation.SetTrigger("SpinAttack");
            _player.CharacterController.stepOffset = 0f;

            IsAttacking = true;
            IsUsingSkill = true;
            CurrentSwordSpinSkill = true;

            if (_spinCoroutine != null)
                StopCoroutine(_spinCoroutine);

            _spinCoroutine = StartCoroutine(EndSpinSkillAfterDelay(5f));

            Debug.Log("스핀 돌아");
        }
    }

    private IEnumerator EndSpinSkillAfterDelay(float delay)
    {
        Debug.Log("코루틴 시작");

        yield return new WaitForSeconds(delay);

        try
        {
            _playerAnimation.SetTrigger("Idle");
            _player.CharacterController.stepOffset = 1f;

            IsUsingSkill = false;
            IsAttacking = false;
            CurrentSwordSpinSkill = false;

            Debug.Log("스핀 멈춰");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EndSpinSkillAfterDelay 예외 발생: {ex.Message}");
        }

        Debug.Log("코루틴 종료");
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
            _swordDashSkill.CurrentSwordDashSkill = false;
            CurrentSwordSpinSkill = true; // 불필요할 수 있음 (이미 UseSkill 내부에서 처리)
        }
    }

    public override void OnSkillEffectPlay()
    {
        // 이 함수가 필요한 경우 확장 가능
    }

    public override void OnSkillAnimationEnd()
    {
        IsAttacking = false;
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if (!IsUsingSkill)
            return;

        float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier;
        IDamageAble damageAble = enemy.GetComponent<IDamageAble>();
        if (damageAble != null)
        {
            Damage damage = new Damage(power, gameObject, 100f, hitDirection);
            damageAble.TakeDamage(damage);
            //Debug.Log($"회전베기 스킬로 {enemy.name}에게 {power}데미지를 입혔다!");
        }
    }
}*/