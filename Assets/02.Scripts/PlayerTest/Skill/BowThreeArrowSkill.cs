using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BowThreeArrowSkill : WeaponSkillBase
{
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;

    [Header("Skill Settings")]
    [SerializeField] private GameObject _arrowPrefab; // 화살 프리팹 할당 (ArrowProjectile 스크립트 없음)
    [SerializeField] private float _skillDamageMultiplier = 1.5f; // 불화살 스킬 데미지 배율
    [SerializeField] private float _cooltime = 5f;

    [Header("Triple Shot Settings")]
    [SerializeField] private int _numberOfArrows = 3; // 발사할 화살 개수
    [SerializeField] private float _arrowSpreadAngle = 10f; // 화살 확산 각도
    [SerializeField] private float _arrowForce = 20f; // 화살 발사 힘
    [SerializeField] private float _arrowLifetime = 5f; // 화살이 자동으로 사라지는 시간 (초)

    [SerializeField] private float _skillDuration = 15f; // 화살 유지 시간

    private float _lastUseTime;
    public override bool IsUsingSkill { get; protected set; }

    private void Awake()
    {
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
        _player = GetComponent<ThirdPersonPlayer>();
    }

    public override void UseSkill()
    {
        if (!IsSkillAvailable())
        {
            Debug.Log("트리플 샷 스킬 쿨타임 안 참");
            return;
        }

        // 활 장착 확인
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Bow)
        {
            Debug.Log("활을 장착해야 트리플 샷 스킬을 사용할 수 있습니다.");
            return;
        }

        IsUsingSkill = true;

        FireTripleArrows();

        _player.CharacterController.stepOffset = 0f;

        StartCoroutine(EndFireArrowSkillAfterDelay(_skillDuration));
        Debug.Log("트리플 샷 공격");
    }

    private IEnumerator EndFireArrowSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;

        //스킬 끝나고 쿨타임
        _lastUseTime = Time.time;
    }

    private void FireTripleArrows()
    {
        if (_arrowPrefab == null)
        {
            Debug.LogError("화살 프리팹이 할당되지 않았습니다. BowFireSkill 인스펙터에서 할당해주세요!");
            return;
        }

        // 플레이어의 전방 방향 (조준점)
        Vector3 fireDirection = _player.transform.forward; // 또는 특정 조준 위치에서 가져옴

        // 화살 확산 시작 각도 계산
        float startAngle = -(_numberOfArrows - 1) * _arrowSpreadAngle / 2f;

        for (int i = 0; i < _numberOfArrows; i++)
        {
            float currentAngle = startAngle + i * _arrowSpreadAngle;
            Quaternion rotationOffset = Quaternion.Euler(0, currentAngle, 0);

            // 현재 화살의 발사 방향
            Vector3 arrowDirection = rotationOffset * fireDirection;

            // 화살 생성 위치 (플레이어 약간 앞, 위)
            Vector3 spawnPosition = _player.transform.position + _player.transform.forward * 1f + _player.transform.up * 1.5f;
            GameObject arrowInstance = Instantiate(_arrowPrefab, spawnPosition, Quaternion.LookRotation(arrowDirection));

            // Rigidbody 가져와서 힘 가하기
            Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(arrowDirection * _arrowForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("화살 프리팹에 Rigidbody 컴포넌트가 없습니다. 물리적 움직임을 위해 추가해주세요!");
            }


            StartCoroutine(DestroyArrowAfterDelay(arrowInstance, _arrowLifetime));
        }
    }

    private IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (arrow != null)
        {
            Destroy(arrow);
        }
    }

    public override bool IsSkillAvailable()
    {
        return Time.time >= _lastUseTime + _cooltime;
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UseSkill();
        }
    }

    // 다음 메서드들은 이 스킬에서는 직접 사용되지 않습니다.
    public override void OnSkillEffectPlay() { /* 불필요 */ }
    public override void OnSkillAnimationEnd() { /* 불필요 */ }
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