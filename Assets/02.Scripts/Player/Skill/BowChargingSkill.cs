using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BowChargingSkill : WeaponSkillBase
{
    [Header("차지 화살 세팅")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float maxChargeTime = 6f;
    public float minShootForce = 20f;
    public float maxShootForce = 60f;
    public float damageMultiplier = 1f;
    public float fullChargeMultiplier = 2f;

    [Header("이펙트")]
    public GameObject chargeEffect;
    //public GameObject fullChargeEffect;

    private float chargeTime = 0f;
    private bool isCharging = false;
    private bool isFullyCharged = false;
    private Animator _playerAnimator;
    private PlayerAttack _playerAttack;

    public override bool IsUsingSkill { get; protected set; }

    private void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
        chargeEffect.SetActive(false);
        _playerAttack = GetComponent<PlayerAttack>();
    }
    public override void Tick()
    {
        base.Tick();

        if (Input.GetKeyDown(KeyCode.E) && !IsUsingSkill && !IsCooldown)
        {
            StartCharge();
        }

        if (isCharging)
        {
            chargeTime += Time.deltaTime;

            if (chargeTime >= maxChargeTime && !isFullyCharged)
            {
                isFullyCharged = true;
                //fullChargeEffect?.SetActive(true);
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && isCharging)
        {
            ReleaseCharge();
        }

        UIManager.instance.UI_CooltimeRefresh(ESkillButton.BowE, CooldownRemaining);
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
        _playerAttack.IsUsingJumpAnim = false;
        _playerAttack.IsMoveSlow = true;

        PlayerSoundController.Instance.PlayLoopSound(PlayerSoundType.BowSkill2Charge);

        _playerAnimator.SetTrigger("Charging");
        chargeEffect.SetActive(true);
    }

    private void ReleaseCharge()
    {
        isCharging = false;
        //chargeEffectGO?.SetActive(false);
        // fullChargeEffect?.SetActive(false);

        PlayerSoundController.Instance.StopLoopSound();
        PlayerSoundController.Instance.PlaySound(PlayerSoundType.BowSkill2Shoot);

        float t = Mathf.Clamp01(chargeTime / maxChargeTime);
        float force = Mathf.Lerp(minShootForce, maxShootForce, t);
        float damageMult = isFullyCharged ? fullChargeMultiplier : damageMultiplier;

        FireChargedArrow(force, damageMult);
        chargeEffect.SetActive(false);
        _playerAnimator.SetTrigger("Idle");
        IsUsingSkill = false;
        _playerAttack.IsUsingJumpAnim = true;
        _playerAttack.IsMoveSlow = false;

        //쿨타임 초기화
        cooldownTimer = cooldownTime;
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


    public override void ResetState()
    {
        // 스킬 상태 초기화
        IsUsingSkill = false;
        isCharging = false;
        isFullyCharged = false;
        chargeTime = 0f;

        // 이펙트 끄기
        if (chargeEffect != null)
            chargeEffect.SetActive(false);
        // fullChargeEffect?.SetActive(false); // 필요 시

        // 애니메이션 리셋
        if (_playerAnimator != null)
        {
            _playerAnimator.ResetTrigger("Charging");
            _playerAnimator.SetTrigger("Idle");
        }

        // 이동 제한 및 점프 가능 복구
        if (_playerAttack != null)
        {
            _playerAttack.IsUsingJumpAnim = true;
            _playerAttack.IsMoveSlow = false;
        }

        Debug.Log("BowChargingSkill 상태 초기화 완료");
    }
}


