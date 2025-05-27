using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordSpinSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;


    //[SerializeField] private float cooltime = 5f;
   // private float lastUseTime;

    public override bool IsUsingSkill { get; protected set; }

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = MyPlayer.GetComponent<Animator>();
        _equipmentController = MyPlayer.GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
    }

    public override void UseSkill()
    {
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

        IsUsingSkill = true;
        //lastUseTime = Time.time;

        _playerAnimation.SetTrigger("SpinAttack");//�ִϸ��̼� ����
        _player.CharacterController.stepOffset = 0f;

        StartCoroutine(EndSpinSkillAfterDelay(5f));

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
    }

    /*public override bool IsSkillAvailable()
    {
        return Time.time >= lastUseTime + cooltime;
    }*/

    public override void Tick()
    {
        Debug.Log("�����ϴ� ��ų �ߵ�" + "Q �� ����");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill();
        }
    }

    public override void OnSkillEffectPlay()
    {
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void OnSkillAnimationEnd()
    {
        // �ִϸ��̼��� ������ �� ó���� ���� �ۼ�
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if(!IsUsingSkill)
        {
            return;
        }

        float power = _equipmentController.GetCurrentWeaponAttackPower();
        IDamageAble damageAble = enemy.GetComponent<IDamageAble>();
        if (damageAble != null)
        {
            Damage damage = new Damage(power, gameObject, 100f, hitDirection);
            damageAble.TakeDamage(damage);
            Debug.Log($"ȸ������ ��ų�� {enemy.name}���� {power}�������� ������!");
        }
    }

}
