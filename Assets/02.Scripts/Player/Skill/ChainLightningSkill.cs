using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ChainLightningSkill : WeaponSkillBase
{
    [Header("Chain Lightning Settings")]
    public float damagePerHit = 40f;
    public int maxChains = 10;
    public float chainRange = 6f;
    public float delayBetweenChains = 0.1f;
    public GameObject lightningEffectHitPrefab;
    public GameObject lightningEffectPrefab;
    public LayerMask enemyLayer;

    private GameObject _player;
    private Animator _playerAnimator;
    private PlayerAttack _playerAttack;
    private bool _isUsingSkill;
    public override bool IsUsingSkill { get => _isUsingSkill; protected set => _isUsingSkill = value; }

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerAnimator = GetComponent<Animator>();
        _playerAttack = GetComponent<PlayerAttack>();
    }

    public override void UseSkill()
    {
        if (IsUsingSkill || IsCooldown) return;


        _playerAnimator.SetTrigger("Skill1");

        _playerAttack.IsMoveSlow = true;
        _playerAttack.IsUsingJumpAnim = false;
    }

    public void SkillStartForAnim()
    {
        IsUsingSkill = true;
        Instantiate(lightningEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

        StartCoroutine(ChainLightningRoutine());
    }
    private void SkillAnimEnd()
    {
        IsUsingSkill = false;
        _playerAttack.IsMoveSlow = false;
        _playerAttack.IsUsingJumpAnim = true;

        //쿨타임 초기화
        cooldownTimer = cooldownTime;
    }

    private IEnumerator ChainLightningRoutine()
    {
        List<GameObject> hitEnemies = new List<GameObject>();
        GameObject currentTarget = FindFirstTarget();

        for (int i = 0; i < maxChains && currentTarget != null; i++)
        {
            if (!hitEnemies.Contains(currentTarget))
            {
                TryDamageEnemy(currentTarget, currentTarget.transform.position - _player.transform.position);
                hitEnemies.Add(currentTarget);
                OnSkillEffectPlay(currentTarget.transform.position);
            }

            yield return new WaitForSeconds(delayBetweenChains);

            currentTarget = FindNextTarget(currentTarget.transform.position, hitEnemies);
        }

    }

    private GameObject FindFirstTarget()
    {
        Vector3 origin = gameObject.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        float maxDistance = 20f;
        float radius = 1.0f;

        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, maxDistance, enemyLayer))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    private GameObject FindNextTarget(Vector3 fromPos, List<GameObject> alreadyHit)
    {
        Collider[] hits = Physics.OverlapSphere(fromPos, chainRange, enemyLayer);
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var col in hits)
        {
            GameObject enemy = col.gameObject;
            if (alreadyHit.Contains(enemy)) continue;

            float dist = Vector3.Distance(fromPos, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if (enemy.TryGetComponent<IDamageAble>(out var target))
        {
            float power = PlayerEquipmentController.Instance.GetCurrentWeaponAttackPower();
            Damage dmg = new Damage(damagePerHit, _player, 0f, hitDirection.normalized);
            target.TakeDamage(dmg);
        }
    }

    public override void OnSkillEffectPlay()
    {
    }

    private void OnSkillEffectPlay(Vector3 position)
    {
        if (lightningEffectPrefab != null)
        {
            Instantiate(lightningEffectPrefab, position + Vector3.up * 1.2f, Quaternion.identity);
        }
    }

    public override void OnSkillAnimationEnd()
    {
    }

    public override void ResetState()
    {
        StopAllCoroutines();
        IsUsingSkill = false;
        _playerAttack.IsMoveSlow = false;
        _playerAttack.IsUsingJumpAnim = true;
    }

    public override void Tick()
    {
        base.Tick();
        UIManager.instance.UI_CooltimeRefresh(ESkillButton.WandQ, CooldownRemaining);
    }
}
