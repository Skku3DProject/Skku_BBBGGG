using UnityEngine;
using System.Collections;

public class SwordDashSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;


    public CapsuleCollider ShieldCollider;

    [SerializeField] private float _skillDamageMultiplier = 1.7f;
    [SerializeField] private float dashDistance = 10f;//거리
    [SerializeField] private float dashSpeed = 50f;//속도

    private SwordSpinSkill _swordSpinSkill;
    public bool CurrentSwordDashSkill;
    public override bool IsUsingSkill { get; protected set; }
    public bool IsAttacking;

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = MyPlayer.GetComponent<Animator>();
        _equipmentController = MyPlayer.GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _swordSpinSkill = MyPlayer.GetComponent<SwordSpinSkill>();

        ShieldCollider.enabled = false;
    }

    public override void UseSkill()
    {
        if(IsAttacking == false)
        {

            if (_equipmentController.GetCurrentEquipType() != EquipmentType.Sword)
                return;

            IsUsingSkill = true;
            CurrentSwordDashSkill = true;

            ShieldCollider.enabled = true;

            _playerAnimation.SetTrigger("DashAttack");
            _player.CharacterController.stepOffset = 0f;

            // 대쉬 이동 시작
            StartCoroutine(DashForward());
            IsAttacking = true;
        }

        
    }

    private IEnumerator DashForward()
    {
        float movedDistance = 0f;
        Vector3 direction = MyPlayer.transform.forward.normalized;
        CharacterController controller = _player.CharacterController;

        while (movedDistance < dashDistance)
        {
            float moveThisFrame = dashSpeed * Time.deltaTime;
            controller.Move(direction * moveThisFrame);
            movedDistance += moveThisFrame;
            yield return null;
        }

        StartCoroutine(EndDashSkillAfterDelay(0.1f));
    }

    private IEnumerator EndDashSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _playerAnimation.SetTrigger("Idle");

        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;
        CurrentSwordDashSkill = false;

        ShieldCollider.enabled = false;
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UseSkill();
            _swordSpinSkill.CurrentSwordSpinSkill = false;
            CurrentSwordDashSkill = true;

        }
    }

    public override void OnSkillEffectPlay() { }

    public override void OnSkillAnimationEnd()
    {
        IsAttacking = false;
        CurrentSwordDashSkill = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if (!IsUsingSkill) return;

        float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier;
        IDamageAble damageAble = enemy.GetComponent<IDamageAble>();

        if (damageAble != null)
        {
            Damage damage = new Damage(power, gameObject, 100f, hitDirection);
            damageAble.TakeDamage(damage);
            Debug.Log($"대쉬 스킬로 {enemy.name}에게 {power} 데미지를 입혔다!");
        }
    }
}

