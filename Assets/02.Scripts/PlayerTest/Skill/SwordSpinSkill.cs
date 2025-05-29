using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordSpinSkill : WeaponSkillBase
{
    //Q�� ��ų �ߵ�

    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;
    [SerializeField] private float _skillDamageMultiplier = 2.5f;

    //[SerializeField] private float cooltime = 5f;
    // private float lastUseTime;
    private SwordDashSkill _swordDashSkill;
    public bool CurrentSwordSpinSkill;
    public override bool IsUsingSkill { get; protected set; }
    public bool IsAttacking;

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
        Debug.Log("�� ���� ���� ����");

        //��Ÿ�� ���� ���ֱ�

        /*if (!IsSkillAvailable())
        {
            Debug.Log("ȸ�� ���� ��Ÿ�� �� á��");
            return;
        }*/

        //�����ϰ� �ִ� �� ���� �ƴϸ� ��ų ��� �Ұ�
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Sword)
        {
            return;
        }

        if (IsAttacking == false)
        {
            IsUsingSkill = true;
            //lastUseTime = Time.time;
            CurrentSwordSpinSkill = true;
            _playerAnimation.SetTrigger("SpinAttack");//�ִϸ��̼� ����
            _player.CharacterController.stepOffset = 0f;

            IsAttacking = true;

            StartCoroutine(EndSpinSkillAfterDelay(5f));
        }

            

    }

    private IEnumerator EndSpinSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ������ Idle ���·� ��ȯ
        _playerAnimation.SetTrigger("Idle"); // �Ǵ� SetBool�� ���� ��ȯ�� ���� ����

        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;

        //��ų ������ ��Ÿ��
        //lastUseTime = Time.time;
        CurrentSwordSpinSkill = false;
    }

    /*public override bool IsSkillAvailable()
    {
        return Time.time >= lastUseTime + cooltime;
    }*/

    public override void Tick()
    {
        //Debug.Log("�����ϴ� ��ų �ߵ�" + "Q �� ����");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
            _swordDashSkill.CurrentSwordDashSkill = false;
            CurrentSwordSpinSkill = true;
        }
    }

    public override void OnSkillEffectPlay()
    {
        //IsUsingSkill = false;
        //_player.CharacterController.stepOffset = 1f;
    }

    public override void OnSkillAnimationEnd()
    {
        // �ִϸ��̼��� ������ �� ó���� ���� �ۼ�
        IsAttacking = false;
        //IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if(!IsUsingSkill)
        {
            return;
        }

        float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier; ;
        IDamageAble damageAble = enemy.GetComponent<IDamageAble>();
        if (damageAble != null)
        {
            Damage damage = new Damage(power, gameObject, 100f, hitDirection);
            damageAble.TakeDamage(damage);
            //Debug.Log($"ȸ������ ��ų�� {enemy.name}���� {power}�������� ������!");
        }
    }

}
