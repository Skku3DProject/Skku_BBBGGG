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
        //쿨타임 조건 없애기

        /*if (!IsSkillAvailable())
        {
            Debug.Log("회전 공격 쿨타임 안 찼음");
            return;
        }*/

        //장착하고 있는 게 검이 아니면 스킬 사용 불가
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Sword)
        {
            return;
        }

        IsUsingSkill = true;
        //lastUseTime = Time.time;

        _playerAnimation.SetTrigger("SpinAttack");//애니메이션 실행
        _player.CharacterController.stepOffset = 0f;

        StartCoroutine(EndSpinSkillAfterDelay(5f));

    }

    private IEnumerator EndSpinSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 강제로 Idle 상태로 전환
        _playerAnimation.SetTrigger("Idle"); // 또는 SetBool로 상태 전환할 수도 있음

        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;

        //스킬 끝나고 쿨타임
        //lastUseTime = Time.time;
    }

    /*public override bool IsSkillAvailable()
    {
        return Time.time >= lastUseTime + cooltime;
    }*/

    public override void Tick()
    {
        Debug.Log("스핀하는 스킬 발동" + "Q 안 누름");

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
        // 애니메이션이 끝났을 때 처리할 내용 작성
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
            Debug.Log($"회전베기 스킬로 {enemy.name}에게 {power}데미지를 입혔다!");
        }
    }

}
