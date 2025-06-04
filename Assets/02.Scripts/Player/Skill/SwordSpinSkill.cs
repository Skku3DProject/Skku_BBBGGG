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

    private Coroutine _spinCoroutine; // �ߺ� ������ �ڷ�ƾ �ڵ�

    private float _baseAnimationDuration = 0.2f;  // �ִϸ��̼� ���� ����
    private float _animationSpeed = 1.0f;         // �ִϸ��̼� ��� �ӵ�
    private int _repeatCount = 15;                  // �ݺ� ��� Ƚ��


    public GameObject SpinVfx;
    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = MyPlayer.GetComponent<Animator>();
        _equipmentController = MyPlayer.GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _swordDashSkill = MyPlayer.GetComponent<SwordDashSkill>();
        _playerAttack = MyPlayer.GetComponent<PlayerAttack>();

        // �ִϸ��̼� �ӵ� ����
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

            Debug.Log("���� ����");

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


        ////��Ÿ�� �ʱ�ȭ
        cooldownTimer = cooldownTime;
        PlayerSoundController.Instance.StopLoopSound();

        Debug.Log("���� ����");
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

        Debug.Log("SwordSpinSkill ���� �ʱ�ȭ");
    }
}