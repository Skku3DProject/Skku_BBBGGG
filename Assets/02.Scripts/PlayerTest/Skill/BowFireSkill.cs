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
    [SerializeField] private float _skillDamageMultiplier = 1.5f;  // 불화살 스킬 데미지 배율

    [SerializeField] private float _skillDuration = 15f; // 불화살 유지 시간
    
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
            Debug.Log("불 화살 공격 쿨타임 안참");
            return;
        }*/

        //장착하고 있는 게 검이 아니면 스킬 사용 불가
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Bow)
        {
            return;
        }

        IsUsingSkill = true;
       // lastUseTime = Time.time;

        // 스킬 사용 시 불 이펙트 활성화
        _fireEffect.SetActive(true);

        //_playerAnimation.SetTrigger("Attack");//애니메이션 실행
        _player.CharacterController.stepOffset = 0f;

        StartCoroutine(EndFireArrowSkillAfterDelay(_skillDuration));
        Debug.Log("불 화살 공격");

    }

    private IEnumerator EndFireArrowSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _fireEffect.SetActive(false);     //  불 이펙트 OFF
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;

        //스킬 끝나고 쿨타임
        //lastUseTime = Time.time;
    }

   /* public override bool IsSkillAvailable()
    {
        return Time.time >= lastUseTime + cooltime;
    }*/

    public override void Tick()
    {
        Debug.Log("불 화살 공격 발동" + "E 안 누름");

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
        // 애니메이션이 끝났을 때 처리할 내용 작성
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
            Debug.Log($"불화살 스킬로 {enemy.name}에게 {power}데미지를 입혔다!");
        }
    }

}
