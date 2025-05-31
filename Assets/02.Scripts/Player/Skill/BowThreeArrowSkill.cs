using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BowThreeArrowSkill : WeaponSkillBase
{
    [Header("차지 화살 세팅")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float maxChargeTime = 2f;
    public float minShootForce = 20f;
    public float maxShootForce = 60f;
    public float damageMultiplier = 1f;
    public float fullChargeMultiplier = 2f;

    [Header("이펙트")]
    public GameObject chargeEffectGO;
    private ParticleSystem[] chargeEffects;
    public GameObject fullChargeEffect;

    private float chargeTime = 0f;
    private bool isCharging = false;
    private bool isFullyCharged = false;
    private Animator _playerAnimator;
    public override bool IsUsingSkill { get; protected set; }

    private void Awake()
    {
        _playerAnimator = GetComponent<Animator>();

        chargeEffects = chargeEffectGO.GetComponents<ParticleSystem>();
        foreach (var effect in chargeEffects)
        {
            effect.Stop();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !IsUsingSkill)
        {
            StartCharge();
        }

        if (isCharging)
        {
            chargeTime += Time.deltaTime;

            if (chargeTime >= maxChargeTime && !isFullyCharged)
            {
                isFullyCharged = true;
                fullChargeEffect?.SetActive(true);
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && isCharging)
        {
            ReleaseCharge();
        }
    }

    public override void UseSkill()
    {
        // 이 함수는 PlayerAttack에서 호출하되, 차지형은 내부적으로 입력으로 처리
    }

    private void StartCharge()
    {
        isCharging = true;
        chargeTime = 0f;
        isFullyCharged = false;
        IsUsingSkill = true;

        _playerAnimator.SetTrigger("Charging");
        foreach (var effect in chargeEffects)
        {
            effect.Play();
        }
    }

    private void ReleaseCharge()
    {
        isCharging = false;
        //chargeEffectGO?.SetActive(false);
       // fullChargeEffect?.SetActive(false);

        float t = Mathf.Clamp01(chargeTime / maxChargeTime);
        float force = Mathf.Lerp(minShootForce, maxShootForce, t);
        float damageMult = isFullyCharged ? fullChargeMultiplier : damageMultiplier;

        FireChargedArrow(force, damageMult);
        foreach (var effect in chargeEffects)
        {
            effect.Stop();
        }
        _playerAnimator.SetTrigger("Idle");
        IsUsingSkill = false;
    }

    private void FireChargedArrow(float force, float damageMult)
    {
        if (arrowPrefab == null || shootPoint == null) return;

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        PlayerArrow arrowScript = arrow.GetComponent<PlayerArrow>();

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = ray.origin + ray.direction * 100f;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            targetPoint = hit.point;

        Vector3 dir = (targetPoint - shootPoint.position).normalized;

        arrow.transform.rotation = Quaternion.LookRotation(dir);
        arrow.transform.Rotate(90f, 0, 0, Space.Self);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * force;

        float basePower = PlayerEquipmentController.Instance.GetCurrentWeaponAttackPower();
        arrowScript?.ArrowInit(basePower, ArrowType.Charging, gameObject);
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        // 충돌 시 화살에서 Damage 처리
    }

    public override void OnSkillEffectPlay() { }

    public override void OnSkillAnimationEnd() { }

    public override void Tick()
    {
        // 필요 시 쿨타임 처리
    }
}


