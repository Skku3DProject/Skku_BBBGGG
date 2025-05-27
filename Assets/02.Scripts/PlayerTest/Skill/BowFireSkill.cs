using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BowFireSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;


    [SerializeField] private GameObject _fireEffect;
    [SerializeField] private float _skillDamageMultiplier = 1.5f;  // ��ȭ�� ��ų ������ ����

    [SerializeField] private float _skillDuration = 15f; // ��ȭ�� ���� �ð�
    
    //[SerializeField] private float cooltime = 5f;
    //private float lastUseTime;

    public override bool IsUsingSkill { get; protected set; }

    private void Awake()
    {
        MyPlayer= GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
    }

    public override void UseSkill()
    {
       /* if (!IsSkillAvailable())
        {
            Debug.Log("�� ȭ�� ���� ��Ÿ�� ����");
            return;
        }*/

        //�����ϰ� �ִ� �� ���� �ƴϸ� ��ų ��� �Ұ�
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Bow)
        {
            return;
        }

        IsUsingSkill = true;
       // lastUseTime = Time.time;

        // ��ų ��� �� �� ����Ʈ Ȱ��ȭ
        _fireEffect.SetActive(true);

        //_playerAnimation.SetTrigger("Attack");//�ִϸ��̼� ����
        _player.CharacterController.stepOffset = 0f;

        StartCoroutine(EndFireArrowSkillAfterDelay(_skillDuration));
        Debug.Log("�� ȭ�� ����");

    }

    private IEnumerator EndFireArrowSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _fireEffect.SetActive(false);     //  �� ����Ʈ OFF
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;

        //��ų ������ ��Ÿ��
        //lastUseTime = Time.time;
    }

   /* public override bool IsSkillAvailable()
    {
        return Time.time >= lastUseTime + cooltime;
    }*/

    public override void Tick()
    {
        Debug.Log("�� ȭ�� ���� �ߵ�" + "E �� ����");

        if (Input.GetKeyDown(KeyCode.E))
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
        if (!IsUsingSkill)
        {
            return;
        }

        float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier;
        //float power = _equipmentController.GetCurrentWeaponAttackPower();
        IDamageAble damageAble = enemy.GetComponent<IDamageAble>();
        if (damageAble != null)
        {
            
            Damage damage = new Damage(power, gameObject, 100f, hitDirection);
            damageAble.TakeDamage(damage);
            Debug.Log($"��ȭ�� ��ų�� {enemy.name}���� {power}�������� ������!");
        }
    }

}
