using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BowThreeArrowSkill : WeaponSkillBase
{
    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;

    

    [Header("Skill Settings")]
    [SerializeField] private GameObject _arrowPrefab; // 화살 프리팹 할당 (ArrowProjectile 스크립트 없음)
    [SerializeField] private float _skillDamageMultiplier = 2.5f; // 스킬 데미지 배율

    [Header("Triple Shot Settings")]
    [SerializeField] private int _numberOfArrows = 3; // 발사할 화살 개수
    [SerializeField] private float _arrowSpreadAngle = 10f; // 화살 확산 각도
    [SerializeField] private float _arrowForce = 20f; // 화살 발사 힘
    [SerializeField] private float _arrowLifetime = 5f; // 화살이 자동으로 사라지는 시간 (초)

    [SerializeField] private float _skillDuration = 15f; // 스킬 유지 시간

    [Header("Arrow Spawn Point")]
    [SerializeField] private Transform _bowArrowSpawnPoint; // 활의 실제 화살 발사 위치
    [SerializeField] private Vector3 _arrowModelRotationOffset = new Vector3(90f, 0f, 0f); // 화살 모델의 초기 회전 보정 (필요시)

    //현재 이 스킬을 사용하고 있는 상태인지 - 다른 스킬 사용하고 있으면 해당 스킬 비활성
    private BowFireSkill _bowFireSkill;
    public bool CurrentThreeArrowSkill;
    public override bool IsUsingSkill { get; protected set; }
    private bool _isAttacking;
   // public bool CanShootNext = true;
    //private float _lastAttackTime;


    private void Awake()
    {
        
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _bowFireSkill = MyPlayer.GetComponent<BowFireSkill>();
    }

    public override void UseSkill()
    {
        if(_bowFireSkill.FireEffect.activeSelf == true)
        {
            _bowFireSkill.FireEffect.SetActive(false);
        }

        /* if (!IsSkillAvailable())
         {
             Debug.Log("트리플 샷 스킬 쿨타임 안 참");
             return;
         }*/

        // 활 장착 확인
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Bow)
        {
            Debug.Log("활을 장착해야 트리플 샷 스킬을 사용할 수 있습니다.");
            return;
        }

        IsUsingSkill = true;

        // FireTripleArrows();

        _player.CharacterController.stepOffset = 0f;

        StartCoroutine(EndFireArrowSkillAfterDelay(_skillDuration));

    }

    private IEnumerator EndFireArrowSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 강제로 Idle 상태로 전환
        _playerAnimation.SetTrigger("Idle"); // 또는 SetBool로 상태 전환할 수도 있음

        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;

        Debug.Log("트리플 샷 유지시간 끝남");
        CurrentThreeArrowSkill = false;

    }

    public void ShootThreeArrow()
    {//
       if (CurrentThreeArrowSkill == true && _isAttacking == false)
        {
            _playerAnimation.SetTrigger("ThreeArrowAttack");
            Debug.Log("트리플 샷 공격");
            _isAttacking = true;

        }
        
    }

    public void FireTripleArrows()
    {

        Debug.Log("애니메이션 중에 쏘는거 호출됨 - 트리플 샷");

        /*if (_arrowPrefab == null)
        {
            Debug.LogError("화살 프리팹이 할당되지 않음");
            return;
        }
        if (_arrowPrefab.GetComponent<Rigidbody>() == null)
        {
            Debug.LogError("할당된 _arrowPrefab에 Rigidbody 컴포넌트가 없습니다! 화살은 물리적 움직임을 가질 수 없습니다.");
            return;
        }
        if (_player == null || Camera.main == null || _bowArrowSpawnPoint == null)
        {
            Debug.LogError("필요한 컴포넌트나 발사 지점이 할당되지 않았습니다. (플레이어, 카메라, 활 발사 지점)");
            return;
        }*/

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f; // 맞는 지점 없으면 카메라 앞 100미터
        }

        // 화살 확산 시작 각도 계산
        float startAngle = -(_numberOfArrows - 1) * _arrowSpreadAngle / 2f;

        for (int i = 0; i < _numberOfArrows; i++)
        {
            float currentAngle = startAngle + i * _arrowSpreadAngle;

            // **핵심 변경 2: 발사 지점을 _bowArrowSpawnPoint로 사용**
            // 발사 방향 계산: (목표 지점 - 활 발사 지점)
            Vector3 baseShootDirection = (targetPoint - _bowArrowSpawnPoint.position).normalized;

            // 확산 각도를 적용하기 위한 Quaternion 생성
            // 기본 발사 방향을 기준으로 좌우로 회전 (Y축 기준)
            Quaternion spreadRotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 finalArrowDirection = spreadRotation * baseShootDirection;

            // 화살 생성 위치는 활의 발사 지점
            GameObject arrowInstance = Instantiate(_arrowPrefab, _bowArrowSpawnPoint.position, Quaternion.LookRotation(finalArrowDirection));

            // **핵심 변경 3: 화살 모델의 초기 회전 보정 (필요시)**
            // 화살 프리팹의 모델이 z축을 바라보지 않을 경우에만 사용합니다.
            // 예를 들어, 화살 모델의 앞쪽이 X축이라면 (0, 90, 0) 같은 값을 사용할 수 있습니다.
            // 인스펙터에서 _arrowModelRotationOffset을 조절하여 가장 자연스러운 회전을 찾으세요.
            arrowInstance.transform.Rotate(_arrowModelRotationOffset, Space.Self);


            Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // **핵심 변경 4: Rigidbody에 힘을 가하기 전 안정화 코루틴 사용**
                StartCoroutine(ApplyForceToArrowAfterDelay(rb, finalArrowDirection.normalized * _arrowForce, _arrowLifetime));
            }
            else
            {
                Debug.LogWarning("화살 프리팹에 Rigidbody 컴포넌트가 없습니다. 물리적 움직임을 위해 추가해주세요!");
                StartCoroutine(DestroyArrowAfterDelay(arrowInstance, _arrowLifetime));
            }

           // CanShootNext = false;
           // _isAttacking = true;
           // _lastAttackTime = Time.time;
        }
    }

    private IEnumerator ApplyForceToArrowAfterDelay(Rigidbody rb, Vector3 force, float lifetime)
    {
        // 1 프레임 또는 다음 물리 업데이트까지 대기하여 Rigidbody가 안정화될 시간을 줍니다.
        // `yield return null;` (1프레임) 또는 `yield return new WaitForFixedUpdate();` (1물리프레임)
        yield return new WaitForFixedUpdate(); // 물리 시뮬레이션 직전에 실행되어 더 안정적일 수 있습니다.

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // 혹시 모를 초기 속도 초기화
            rb.angularVelocity = Vector3.zero; // 혹시 모를 초기 회전 초기화
            rb.AddForce(force, ForceMode.VelocityChange); // 힘 적용
        }

        _isAttacking = false;
        // 화살 수명 관리
        StartCoroutine(DestroyArrowAfterDelay(rb.gameObject, lifetime));
    }

    public override void OnSkillAnimationEnd()
    {
        Debug.Log("3개 활 쏘는 애니메이션 하고 실행될 거");

        //CanShootNext = true;
        _isAttacking = false;
    }



    private IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (arrow != null)
        {
            Destroy(arrow);
        }
    }


    public override void Tick()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            UseSkill();
            _bowFireSkill.CurrentArrowFireSkill = false;
            CurrentThreeArrowSkill = true;
        }
    }


    public override void OnSkillEffectPlay()
    {
        //IsUsingSkill = false;
        //_player.CharacterController.stepOffset = 1f;
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
            Debug.Log($"화살 3개 스킬로 {enemy.name}에게 {power}데미지를 입혔다!");
        }
    }
}


