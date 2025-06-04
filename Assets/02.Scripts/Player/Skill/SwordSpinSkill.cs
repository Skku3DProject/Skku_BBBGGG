using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class SwordSpinSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;
    private PlayerAttack _playerAttack;
    [SerializeField] private float _skillDamageMultiplier = 0.4f;

    private SwordDashSkill _swordDashSkill;
    public bool CurrentSwordSpinSkill;
    public override bool IsUsingSkill { get; protected set; }
    public bool IsAttacking;

    private Coroutine _spinCoroutine; // 중복 방지용 코루틴 핸들

    private float _baseAnimationDuration = 0.2f;  // 애니메이션 원래 길이
    private float _animationSpeed = 1.0f;         // 애니메이션 재생 속도
    private int _repeatCount = 15;                  // 반복 재생 횟수


    public GameObject SpinVfx;
    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = MyPlayer.GetComponent<Animator>();
        _equipmentController = MyPlayer.GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _swordDashSkill = MyPlayer.GetComponent<SwordDashSkill>();
        _playerAttack = MyPlayer.GetComponent<PlayerAttack>();

        // 애니메이션 속도 설정
        _playerAnimation.SetFloat("SpinAttackSpeed", _animationSpeed);
    }

    public override void UseSkill()
    {

        if (IsCooldown) return;

        if (!IsAttacking && !IsUsingSkill && !CurrentSwordSpinSkill)
        {


            IsAttacking = true;
            IsUsingSkill = true;
            CurrentSwordSpinSkill = true;

            _player.CharacterController.stepOffset = 0f;

            if (_spinCoroutine != null)
                StopCoroutine(_spinCoroutine);

            _spinCoroutine = StartCoroutine(PlaySpinAnimationMultipleTimes(_repeatCount, _baseAnimationDuration, _animationSpeed));

            _playerAttack.IsUsingJumpAnim = false;

            SpinVfx?.SetActive(true);
            PlayerSoundController.Instance.PlayLoopSound(PlayerSoundType.SwoardSKill1);

            Debug.Log("스핀 시작");

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
        _playerAttack.IsUsingJumpAnim = true;
        SpinVfx?.SetActive(false);


        ////쿨타임 초기화
        cooldownTimer = cooldownTime;
        PlayerSoundController.Instance.StopLoopSound();

        Debug.Log("스핀 종료");
    }

    public override void Tick()
    {
        base.Tick();
        Debug.Log(CooldownRemaining);
        UIManager.instance.UI_CooltimeRefresh(ESkillButton.SwordQ, CooldownRemaining);
    }


    public override void OnSkillEffectPlay()
    {
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
            Damage damage = new Damage(power, gameObject, 20f, hitDirection);
            damageAble.TakeDamage(damage);
        }
    }

    public override void ResetState()
    {
        if (_spinCoroutine != null)
        {
            StopCoroutine(_spinCoroutine);
            _spinCoroutine = null;
        }

        IsUsingSkill = false;
        IsAttacking = false;
        CurrentSwordSpinSkill = false;

        if (_player != null)
            _player.CharacterController.stepOffset = 1f;

        if (_playerAttack != null)
            _playerAttack.IsUsingJumpAnim = true;

        if (SpinVfx != null)
            SpinVfx.SetActive(false);

        if (_playerAnimation != null)
        {
            _playerAnimation.ResetTrigger("SpinAttack");
            _playerAnimation.SetTrigger("Idle");
        }

        PlayerSoundController.Instance.StopLoopSound();

        Debug.Log("SwordSpinSkill 상태 초기화");
    }
}