using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordDashSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;

    [SerializeField] private float _skillDamageMultiplier = 1.7f;
    [SerializeField] private float dashDistance = 10f;//거리
    [SerializeField] private float dashSpeed = 50f;//속도

    public bool CurrentSwordDashSkill;
    public override bool IsUsingSkill { get; protected set; }
    public bool IsAttacking;

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = MyPlayer.GetComponent<Animator>();
        _equipmentController = MyPlayer.GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();


    }

    public override void UseSkill()
    {
        if(IsAttacking == false)
        {

            if (_equipmentController.GetCurrentEquipType() != EquipmentType.Sword)
                return;

            IsUsingSkill = true;
            CurrentSwordDashSkill = true;

            _playerAnimation.SetTrigger("DashAttack");
            _player.CharacterController.stepOffset = 0f;

            // 대쉬 이동 시작
            StartCoroutine(DashForward());
            IsAttacking = true;
        }

        
    }

    private IEnumerator DashForward()
    {
        float moved = 0f;
        Vector3 dir = MyPlayer.transform.forward.normalized;
        CharacterController controller = _player.CharacterController;

        float hitRadius = 1.2f;
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

        while (moved < dashDistance)
        {
            float move = dashSpeed * Time.deltaTime;
            controller.Move(dir * move);
            moved += move;

            Vector3 center = MyPlayer.transform.position + dir * 2f + Vector3.up * 0.7f;// 캐릭터 중점이 발이라 y오프셋값 추가
            Collider[] hits = Physics.OverlapSphere(center, hitRadius, enemyMask);
            BlockSystem.DamageBlocksInRadius(center, hitRadius, 10);
            foreach (var col in hits)
            {
                GameObject enemy = col.gameObject;
                if (alreadyHit.Contains(enemy)) continue;

                TryDamageEnemy(enemy, (enemy.transform.position - MyPlayer.transform.position).normalized);
                alreadyHit.Add(enemy);
            }

            yield return null;
        }

        yield return StartCoroutine(EndChargeSkillAfterDelay(0.1f));
    }

    private IEnumerator EndChargeSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _playerAnimation.SetTrigger("Idle");

        IsUsingSkill = false;
        CurrentSwordDashSkill = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void Tick()
    {
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
           // Debug.Log($"대쉬 스킬로 {enemy.name}에게 {power} 데미지를 입혔다!");
        }
    }

    void OnDrawGizmos()
    {
        if (IsUsingSkill)
        {
            Gizmos.color = Color.red;
            Vector3 center = MyPlayer.transform.position + MyPlayer.transform.forward * 2.0f + Vector3.up * 0.5f; 
            Gizmos.DrawWireSphere(center, 1f);
        }
    }
}

