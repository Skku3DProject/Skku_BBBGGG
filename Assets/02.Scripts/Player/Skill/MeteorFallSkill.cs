using System.Collections.Generic;
using UnityEngine;

public class MeteorFallSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _animator;
    private PlayerAttack _playerAttack;

    [Header("Meteor Settings")]
    public GameObject meteorPrefab;
    public float meteorDamage = 100f;
    [SerializeField] private int meteorCount = 3;
    [SerializeField] private float spawnSpreadRadius = 2f;

    [Header("이펙트")]
    public GameObject castEffectPrefab;
    public Transform EffectPos;

    private Transform spawnPoint;
    private bool _isUsingSkill;
    public override bool IsUsingSkill { get => _isUsingSkill; protected set => _isUsingSkill = value; }

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _animator = MyPlayer.GetComponent<Animator>();
        _playerAttack = MyPlayer.GetComponent<PlayerAttack>();
    }

    public override void UseSkill()
    {
        if (IsUsingSkill || IsCooldown || PlayerEquipmentController.Instance.GetCurrentEquipType() != EquipmentType.Magic)
            return;
        cooldownTimer = cooldownTime;

        _animator.SetTrigger("Skill2"); // ���� Ʈ����

        IsUsingSkill = true;
        _playerAttack.IsMoveSlow = true;
        _playerAttack.IsUsingJumpAnim = false;

    }

    private void SpawnMeteor()
    {
        Vector3 baseSpawnPosition = MyPlayer.transform.position + Vector3.up * 5f + MyPlayer.transform.forward * 2f;

        List<Transform> enemyTargets = FindRandomEnemies(meteorCount);

        for (int i = 0; i < meteorCount; i++)
        {
            // 랜덤 위치 오프셋 (수평 방향만 퍼지게)
            Vector3 randomOffset = Random.insideUnitSphere;
            randomOffset.y = 0;
            Vector3 spawnPos = baseSpawnPosition + randomOffset * spawnSpreadRadius;

            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            Transform target = (i < enemyTargets.Count) ? enemyTargets[i] : null;

            if (meteor != null)
            {
                var proj = meteor.GetComponent<Meteor>();
                if (proj != null)
                {
                    proj.Init(meteorDamage, MyPlayer, target);
                }
            }
        }

        IsUsingSkill = false;
        _playerAttack.IsMoveSlow = false;
        _playerAttack.IsUsingJumpAnim = true;


        OnSkillEffectPlay(); // 스킬 시전 이펙트 재생
    }
    private List<Transform> FindRandomEnemies(int count)
    {
        float detectRadius = 30f;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider[] hits = Physics.OverlapSphere(MyPlayer.transform.position, detectRadius, enemyLayer);

        List<Transform> candidates = new List<Transform>();
        foreach (var hit in hits)
        {
            candidates.Add(hit.transform);
        }

        List<Transform> results = new List<Transform>();
        for (int i = 0; i < count; i++)
        {
            if (candidates.Count == 0) break;

            // 중복 허용: 랜덤으로 하나 선택
            int index = Random.Range(0, candidates.Count);
            results.Add(candidates[index]);
        }

        return results;
    }
    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if (!IsUsingSkill) return;

        if (enemy.TryGetComponent<IDamageAble>(out var target))
        {
            float power = meteorDamage; 
            Damage dmg = new Damage(power, gameObject, 0f, hitDirection.normalized);
            target.TakeDamage(dmg);
        }
    }

    public override void OnSkillEffectPlay()
    {
        if (castEffectPrefab != null)
        {
            Instantiate(castEffectPrefab, EffectPos.position, Quaternion.identity);
        }
    }

    public override void OnSkillAnimationEnd()
    {

    }

    public override void Tick()
    {
        base.Tick();
        UIManager.instance.UI_CooltimeRefresh(ESkillButton.WandE, CooldownRemaining);
    }

    public override void ResetState()
    {
        IsUsingSkill = false;
        if (_playerAttack != null)
        {
            _playerAttack.IsMoveSlow = false;
            _playerAttack.IsUsingJumpAnim = true;
        }
    }
}
