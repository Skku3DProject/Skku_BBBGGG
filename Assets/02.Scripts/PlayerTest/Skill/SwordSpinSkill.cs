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

    private Coroutine _spinCoroutine; // �ߺ� ������ �ڷ�ƾ �ڵ�

    private float _baseAnimationDuration = 0.2f;  // �ִϸ��̼� ���� ����
    private float _animationSpeed = 1.0f;         // �ִϸ��̼� ��� �ӵ�
    private int _repeatCount = 15;                  // �ݺ� ��� Ƚ��

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = MyPlayer.GetComponent<Animator>();
        _equipmentController = MyPlayer.GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _swordDashSkill = MyPlayer.GetComponent<SwordDashSkill>();

        // �ִϸ��̼� �ӵ� ����
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

            Debug.Log("���� ����");

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

        Debug.Log("���� ����");
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
            _swordDashSkill.CurrentSwordDashSkill = false;
            // CurrentSwordSpinSkill = true; // UseSkill ������ ó���ϹǷ� �ߺ� ���� ����
        }
    }

    public override void OnSkillEffectPlay()
    {
        // �ʿ�� Ȯ�� ����
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
            Debug.Log($"ȸ������ ��ų�� {enemy.name}���� {power}�������� ������!");
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

    private Coroutine _spinCoroutine; // �ߺ� ������ �ڷ�ƾ �ڵ�

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

            Debug.Log("���� ����");
        }
    }

    private IEnumerator EndSpinSkillAfterDelay(float delay)
    {
        Debug.Log("�ڷ�ƾ ����");

        yield return new WaitForSeconds(delay);

        try
        {
            _playerAnimation.SetTrigger("Idle");
            _player.CharacterController.stepOffset = 1f;

            IsUsingSkill = false;
            IsAttacking = false;
            CurrentSwordSpinSkill = false;

            Debug.Log("���� ����");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EndSpinSkillAfterDelay ���� �߻�: {ex.Message}");
        }

        Debug.Log("�ڷ�ƾ ����");
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
            _swordDashSkill.CurrentSwordDashSkill = false;
            CurrentSwordSpinSkill = true; // ���ʿ��� �� ���� (�̹� UseSkill ���ο��� ó��)
        }
    }

    public override void OnSkillEffectPlay()
    {
        // �� �Լ��� �ʿ��� ��� Ȯ�� ����
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
            //Debug.Log($"ȸ������ ��ų�� {enemy.name}���� {power}�������� ������!");
        }
    }
}*/