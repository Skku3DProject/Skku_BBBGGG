using UnityEngine;

public class SwordAttack : WeaponAttackBase
{
    private ThirdPersonPlayer _player;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;

    public override bool IsAttacking { get; protected set; }
    private int _comboStep = 0;
    private bool _canNextCombo = true;
    private bool _nextComboQueued = false;
    private float _comboResetTime = 1.0f;
    private float _lastAttackTime;

    [Header("Attack Effects")]
    public ParticleSystem SwordSlashEffect;

    private int _currentAttackIndex;

    void Awake()
    {
        _player = GetComponent<ThirdPersonPlayer>();
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
    }

    void Update()
    {
        //if (IsAttacking && Time.time - _lastAttackTime > _comboResetTime)
        //{
        //    ResetCombo();
        //}
    }

    public override void Attack()
    {
        if (!IsAttacking)
        {
            _comboStep = 0;
            IsAttacking = true;
            PlayComboAttack(_comboStep);
            _lastAttackTime = Time.time;
        }
        else if (_canNextCombo)
        {
            _nextComboQueued = true;
            _lastAttackTime = Time.time;
        }
    }

    private void PlayComboAttack(int index)
    {
        _playerAnimation.SetFloat("AttackIndex", index);
        _playerAnimation.SetTrigger("Attack");

        _currentAttackIndex = index;
        _canNextCombo = false;
        _nextComboQueued = false;
        _player.CharacterController.stepOffset = 0f;
    }

    private void ResetCombo()
    {
        IsAttacking = false;
        _comboStep = 0;
        _playerAnimation.SetFloat("AttackIndex", 0);
        _canNextCombo = false;
        _nextComboQueued = false;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 dir)
    {
        if (!IsAttacking) return;

        var currentEquipType = _equipmentController.GetCurrentEquipType();

        float attackPower = _equipmentController.GetCurrentWeaponAttackPower();

        IDamageAble damageable = enemy.GetComponent<IDamageAble>();
        if (damageable != null)
        {
            Damage damage = new Damage(attackPower, gameObject, 10f, dir);
            damageable.TakeDamage(damage);
            Debug.Log($"���� ����: {enemy.name}���� {attackPower} �������� ��");
        }
        else
        {
            Debug.LogWarning($"IDamageAble ������Ʈ�� {enemy.name}���� ã�� �� ����");
        }
    }

    public override void OnAttackEffectPlay()
    {
        SwordSlashEffect?.Play();
    }

    public override void OnAttackAnimationEnd()
    {
        _player.CharacterController.stepOffset = 1f;

        if (_nextComboQueued)
        {
            _comboStep++;
            if (_comboStep > 3)
            {
                ResetCombo();
                return;
            }
            PlayComboAttack(_comboStep);
            _lastAttackTime = Time.time;
        }
        else
        {
            ResetCombo();
        }
    }

    public override void EnableComboInput()
    {
        _canNextCombo = true;
    }

    public override void Tick()
    {
        base.Tick();
        Debug.Log("aim");
        // ���� �޺� ���� �ð� ��� üũ
        if (IsAttacking && Time.time - _lastAttackTime > _comboResetTime)
        {
            ResetCombo();
        }

        // ���� �Է� ó�� (��: ���콺 ��Ŭ��)
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }
}
