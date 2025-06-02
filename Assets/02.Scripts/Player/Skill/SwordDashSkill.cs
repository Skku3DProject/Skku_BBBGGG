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
    [SerializeField] private float dashDistance = 10f;//�Ÿ�
    [SerializeField] private float dashSpeed = 50f;//�ӵ�

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

            // �뽬 �̵� ����
            StartCoroutine(DashForward());
            IsAttacking = true;


            //��Ÿ�� �ʱ�ȭ
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

            Vector3 center = MyPlayer.transform.position + dir * 2f + Vector3.up * 0.7f;// ĳ���� ������ ���̶� y�����°� �߰�
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
           // Debug.Log($"�뽬 ��ų�� {enemy.name}���� {power} �������� ������!");
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
        // ��ų ���� ����
        IsUsingSkill = false;
        IsAttacking = false;
        CurrentSwordDashSkill = false;

        // VFX ��Ȱ��ȭ
        if (DashVfx != null)
            DashVfx.SetActive(false);

        // ĳ���� stepOffset ����
        if (_player != null)
            _player.CharacterController.stepOffset = 1f;

        // �ִϸ��̼� ���� ����
        if (_playerAnimation != null)
        {
            _playerAnimation.ResetTrigger("DashAttack");
            _playerAnimation.SetTrigger("Idle");
        }

        // �ڷ�ƾ�� ���� �־��ٸ� ����
        StopAllCoroutines(); // ���� �ڷ�ƾ ������ �ٲٴ� �͵� ����
    }
}

