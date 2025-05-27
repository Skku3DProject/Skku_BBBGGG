using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BowThreeArrowSkill : WeaponSkillBase
{

    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;

    [Header("Skill Settings")]
    [SerializeField] private GameObject _arrowPrefab; // 화살 프리팹 (SimpleArrowHit 스크립트 포함 권장)
    [SerializeField] private float _skillDamageMultiplier = 1.5f; // 스킬 데미지 배율
    [SerializeField] private float _cooltime = 5f; // 스킬 쿨타임
    [SerializeField] private float _skillActiveDuration = 5f; // R키를 눌렀을 때 스킬이 활성화되는 시간

    [Header("Triple Shot Settings")]
    [SerializeField] private int _maxArrowsPerSkill = 3; // 스킬 활성화 시 쏠 수 있는 최대 화살 개수
    [SerializeField] private float _arrowSpreadAngle = 10f; // 화살 확산 각도
    [SerializeField] private float _arrowForce = 20f; // 화살 발사 힘
    [SerializeField] private float _arrowLifetime = 5f; // 화살이 자동으로 사라지는 시간 (초)

    private float _lastUseTime; // 마지막 쿨타임 시작 시간
    private float _skillActivationTime; // 스킬 활성화 시작 시간
    private int _arrowsFiredInCurrentSkill; // 현재 스킬 활성화 상태에서 발사한 화살 개수

    public override bool IsUsingSkill { get; protected set; } // 이 변수는 이제 스킬 활성화 상태를 나타냅니다.

    private void Awake()
    {
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
        _player = GetComponent<ThirdPersonPlayer>();
    }

    // Tick 메서드는 매 프레임 실행됩니다.
    public override void Tick()
    {
        // 1. R 키를 눌러 스킬 활성화 시도
        if (Input.GetKeyDown(KeyCode.R)) // E 대신 R 키로 변경
        {
            TryActivateSkill();
        }

        // 2. 스킬이 활성화된 상태에서 마우스 왼쪽 버튼 누르면 화살 발사
        if (IsUsingSkill) // 스킬이 활성화된 상태라면
        {
            // 스킬 활성화 시간이 만료되었는지 확인
            if (Time.time >= _skillActivationTime + _skillActiveDuration)
            {
                EndSkillState();
                return; // 스킬 시간 끝나면 더 이상 화살 못 쏨
            }

            // 남은 화살이 있고, 마우스 왼쪽 버튼을 누르면
            if (_arrowsFiredInCurrentSkill < _maxArrowsPerSkill && Input.GetMouseButtonDown(0))
            {
                FireSingleArrow();
                _arrowsFiredInCurrentSkill++; // 화살 발사 횟수 증가

                // 모든 화살을 다 쏘면 스킬 상태 종료
                if (_arrowsFiredInCurrentSkill >= _maxArrowsPerSkill)
                {
                    EndSkillState();
                }
            }
        }
    }

    // 스킬 활성화 시도 메서드
    private void TryActivateSkill()
    {
        if (!IsSkillAvailable())
        {
            Debug.Log("트리플 샷 스킬 쿨타임이 아직 남았습니다.");
            return;
        }

        // 활 장착 확인
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Bow)
        {
            Debug.Log("활을 장착해야 트리플 샷 스킬을 사용할 수 있습니다.");
            return;
        }

        // 스킬 활성화
        IsUsingSkill = true; // 스킬 활성화 상태로 전환
        _skillActivationTime = Time.time; // 스킬 활성화 시작 시간 기록
        _arrowsFiredInCurrentSkill = 0; // 발사한 화살 개수 초기화
        Debug.Log($"트리플 샷 스킬 활성화! {_skillActiveDuration}초 동안 화살을 쏠 수 있습니다.");
    }

    // 개별 화살 발사 메서드
    private void FireSingleArrow()
    {
        if (_arrowPrefab == null)
        {
            Debug.LogError("화살 프리팹이 할당되지 않았습니다. BowFireSkill 인스펙터에서 할당해주세요!");
            return;
        }

        // 플레이어의 전방 방향 (조준점)
        Vector3 fireDirection = _player.transform.forward;

        // 화살 확산 각도 계산 (가운데 화살 기준)
        float currentArrowAngle = -(_maxArrowsPerSkill - 1) * _arrowSpreadAngle / 2f + (_arrowsFiredInCurrentSkill * _arrowSpreadAngle);
        Quaternion rotationOffset = Quaternion.Euler(0, currentArrowAngle, 0);

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

        // 화살이 일정 시간 후 파괴되도록 코루틴 시작
        StartCoroutine(DestroyArrowAfterDelay(arrowInstance, _arrowLifetime));

        // 애니메이션 트리거 (화살 발사 애니메이션)
        _playerAnimation.SetTrigger("Attack");
    }

    // 스킬 상태 종료 메서드
    private void EndSkillState()
    {
        IsUsingSkill = false; // 스킬 비활성화
        _lastUseTime = Time.time; // 쿨타임 시작
        _arrowsFiredInCurrentSkill = 0; // 발사 횟수 초기화
        Debug.Log("트리플 샷 스킬 비활성화 및 쿨타임 시작.");
    }

    // 스킬 사용 가능 여부 (쿨타임 체크)
    public override bool IsSkillAvailable()
    {
        return Time.time >= _lastUseTime + _cooltime;
    }

    // 개별 화살 파괴 코루틴
    private IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (arrow != null)
        {
            Destroy(arrow);
        }
    }

    // 이 스킬에서는 직접 사용되지 않습니다.
    public override void UseSkill() { /* TryActivateSkill()로 대체 */ }
    public override void OnSkillEffectPlay() { /* 불필요 */ }
    public override void OnSkillAnimationEnd() { /* 불필요 */ }
     public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if (!IsUsingSkill)
        {
            return;
        }

        float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier;
        IDamageAble damageAble = enemy.GetComponent<IDamageAble>();
        if (damageAble != null)
        {

            Damage damage = new Damage(power, gameObject, 100f, hitDirection);
            damageAble.TakeDamage(damage);
            Debug.Log($"화살 3개 {enemy.name}에게 {power}데미지를 입혔다!");
        }
    }
    //타이밍 제외 원하는대로 작동 함
    /*private Animator _playerAnimation;
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
        _lastUseTime = Time.time; // 쿨타임 시작

        // 활 발사 애니메이션 재생
        _playerAnimation.SetTrigger("Attack"); // "Attack"이 활 발사 애니메이션 트리거라고 가정

        FireTripleArrows();

        // 스킬은 화살 발사와 동시에 사용 완료로 간주
        IsUsingSkill = false;
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

    public override void OnSkillEffectPlay()
    {

    }

    public override void OnSkillAnimationEnd()
    {

    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        if (!IsUsingSkill)
        {
            return;
        }

        float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier;
        IDamageAble damageAble = enemy.GetComponent<IDamageAble>();
        if (damageAble != null)
        {

            Damage damage = new Damage(power, gameObject, 100f, hitDirection);
            damageAble.TakeDamage(damage);
            Debug.Log($"화살 3개 {enemy.name}에게 {power}데미지를 입혔다!");
        }
    }*/
}