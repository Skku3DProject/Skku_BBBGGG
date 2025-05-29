using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BowFireSkill : WeaponSkillBase
{

    [Header("Skill Settings")]
    [SerializeField] private GameObject _arrowPrefab; // 화살 프리팹 할당 (ArrowProjectile 스크립트 없음)
    [SerializeField] private float _skillDamageMultiplier = 3.5f; // 스킬 데미지 배율
    [SerializeField] private Transform _bowArrowSpawnPoint; // 활의 실제 화살 발사 위치
    [SerializeField] private float _arrowLifetime = 5f; // 화살이 자동으로 사라지는 시간 (초)
    [SerializeField] private float _arrowForce = 20f; // 화살 발사 힘

    public GameObject MyPlayer;
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;


    public GameObject FireEffect;

    [SerializeField] private float _skillDuration = 15f; // 불화살 유지 시간


    //현재 이 스킬을 사용하고 있는 상태인지 - 다른 스킬 사용하고 있으면 해당 스킬 비활성
    private BowThreeArrowSkill _bowThreeArrowSkill;
    public bool CurrentArrowFireSkill;

    //[SerializeField] private float cooltime = 5f;
    //private float lastUseTime;

    public override bool IsUsingSkill { get; protected set; }
    private bool _isAttacking;
    //private bool _canShootNext = true;
    //private float _lastAttackTime;


    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
        _bowThreeArrowSkill = MyPlayer.GetComponent<BowThreeArrowSkill>();
    }

    public override void UseSkill()
    {
        /* if (!IsSkillAvailable())
         {
             Debug.Log("불 화살 공격 쿨타임 안참");
             return;
         }*/

        //장착하고 있는 게 검이 아니면 스킬 사용 불가
        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Bow)
        {
            return;
        }

        IsUsingSkill = true;
        // lastUseTime = Time.time;

        // 스킬 사용 시 불 이펙트 활성화
        FireEffect.SetActive(true);

        //_playerAnimation.SetTrigger("Attack");//애니메이션 실행
        _player.CharacterController.stepOffset = 0f;

        StartCoroutine(EndFireArrowSkillAfterDelay(_skillDuration));
      //  Debug.Log("불 화살 공격");

    }

    private IEnumerator EndFireArrowSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 강제로 Idle 상태로 전환
        _playerAnimation.SetTrigger("Idle"); // 또는 SetBool로 상태 전환할 수도 있음

        FireEffect.SetActive(false);     //  불 이펙트 OFF
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;

       // Debug.Log("불화살 유지시간 끝남");
        //스킬 끝나고 쿨타임
        //lastUseTime = Time.time;
        CurrentArrowFireSkill = false;
    }

    public void ShootFireArrow()
    {
        if (CurrentArrowFireSkill == true && _isAttacking == false)
        {
            _playerAnimation.SetTrigger("FireArrowAttack");
            //Debug.Log("불화살 공격");
            _isAttacking = true;
        }

    }

    public void ShootArrow()
    {
        //Debug.Log("애니메이션 중에 쏘는거 호출됨 - 불화살");

        if (_arrowPrefab == null || _bowArrowSpawnPoint == null || Camera.main == null) return;

        PlayerArrow arrow = Instantiate(_arrowPrefab, _bowArrowSpawnPoint.position, Quaternion.identity).GetComponent<PlayerArrow>();
        arrow.SetAttackPower(PlayerEquipmentController.Instance.GetCurrentWeaponAttackPower());

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb == null) return;

        // 카메라 화면 중앙에서 Raycast 쏘기
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            // 맞는 지점 없으면 카메라 앞 100미터 방향으로 설정
            targetPoint = ray.origin + ray.direction * 100f;
        }

        // 발사 방향 계산 (화살 위치 → 목표 지점)
        Vector3 shootDirection = (targetPoint - _bowArrowSpawnPoint.position).normalized;

        // 화살 회전 설정 (발사 방향 기준)
        arrow.transform.rotation = Quaternion.LookRotation(shootDirection);
        arrow.transform.Rotate(90f, 0f, 0f, Space.Self);
        // 화살 속도 설정
        //rb.linearVelocity = shootDirection * _arrowForce;
        if (rb != null)
        {
            // **핵심 변경 4: Rigidbody에 힘을 가하기 전 안정화 코루틴 사용**
            StartCoroutine(ApplyForceToArrowAfterDelay(rb, shootDirection.normalized * _arrowForce, _arrowLifetime));
        }

        // _canShootNext = false;
        //IsAttacking = true;
        //_lastAttackTime = Time.time;
    }

    /* public override bool IsSkillAvailable()
     {
         return Time.time >= lastUseTime + cooltime;
     }*/

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
       // StartCoroutine(DestroyArrowAfterDelay(rb.gameObject, lifetime));
    }

  /*  private IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (arrow != null)
        {
            Destroy(arrow);
        }
    }*/

    public override void Tick()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {

            UseSkill();
            _bowThreeArrowSkill.CurrentThreeArrowSkill = false;
            CurrentArrowFireSkill = true;
        }
    }

    public override void OnSkillEffectPlay()
    {
        //     IsUsingSkill = false;
        //_player.CharacterController.stepOffset = 1f;
    }

    public override void OnSkillAnimationEnd()
    {
        // 애니메이션이 끝났을 때 처리할 내용 작성
        _isAttacking = false;
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
            Debug.Log($"불화살 스킬로 {enemy.name}에게 {power}데미지를 입혔다!");
        }
    }

}
