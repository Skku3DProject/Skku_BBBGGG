using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SwordDashSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;


    public GameObject DashVfx;

    [SerializeField] private float _skillDamageMultiplier = 0.7f;
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
        if (IsCooldown) return;


        if (IsAttacking == false)
        {

            if (_equipmentController.GetCurrentEquipType() != EquipmentType.Sword)
                return;

            IsUsingSkill = true;
            CurrentSwordDashSkill = true;

            PlayerSoundController.Instance.PlaySound(PlayerSoundType.SwoardSkill2);


            _playerAnimation.SetTrigger("DashAttack");
            DashVfx?.SetActive(true);
            _player.CharacterController.stepOffset = 0f;

            // 대쉬 이동 시작
            StartCoroutine(DashForward());
            IsAttacking = true;


            //쿨타임 초기화
            cooldownTimer = cooldownTime;
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
        DashVfx?.SetActive(false);
        IsUsingSkill = false;
        CurrentSwordDashSkill = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void Tick()
    {
        base.Tick();
        UIManager.instance.UI_CooltimeRefresh(ESkillButton.SwordE, CooldownRemaining);
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

    public override void ResetState()
    {
        // 스킬 상태 리셋
        IsUsingSkill = false;
        IsAttacking = false;
        CurrentSwordDashSkill = false;

        // VFX 비활성화
        if (DashVfx != null)
            DashVfx.SetActive(false);

        // 캐릭터 stepOffset 복구
        if (_player != null)
            _player.CharacterController.stepOffset = 1f;

        // 애니메이션 상태 복구
        if (_playerAnimation != null)
        {
            _playerAnimation.ResetTrigger("DashAttack");
            _playerAnimation.SetTrigger("Idle");
        }

        // 코루틴이 돌고 있었다면 종료
        StopAllCoroutines(); // 단일 코루틴 변수로 바꾸는 것도 가능
    }
}

