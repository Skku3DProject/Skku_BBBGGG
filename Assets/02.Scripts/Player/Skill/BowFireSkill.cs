using UnityEngine;
using System.Collections;

public class BowFireSkill : WeaponSkillBase
{
    public GameObject FireEffect;

    public GameObject MyPlayer;
    private Animator _playerAnimator;
    private PlayerEquipmentController _equipmentController;
    private ThirdPersonPlayer _player;

    [Header("Skill Settings")]
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float _skillDamageMultiplier = 2f;
    [SerializeField] private float _arrowForce = 20f;
    [SerializeField] private float _arrowLifetime = 5f;
    [SerializeField] private float _skillDuration = 10f;

    [Header("Arrow Spawn Point")]
    [SerializeField] private Transform _bowArrowSpawnPoint;
    [SerializeField] private Vector3 _arrowModelRotationOffset = new Vector3(90f, 0f, 0f);

    public bool CurrentArrowFireSkill;
    public override bool IsUsingSkill { get; protected set; }
    private bool _isAttacking;

    private void Awake()
    {
        MyPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerAnimator = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
        _player = MyPlayer.GetComponent<ThirdPersonPlayer>();
    }

    public override void UseSkill()
    {
        FireEffect.SetActive(true);

        if (_equipmentController.GetCurrentEquipType() != EquipmentType.Bow)
        {
            return;
        }

        IsUsingSkill = true;
        CurrentArrowFireSkill= true;
        _player.CharacterController.stepOffset = 0f;
        StartCoroutine(EndSkillAfterDelay(_skillDuration));
    }

    private IEnumerator EndSkillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsUsingSkill = false;
        _player.CharacterController.stepOffset = 1f;
        CurrentArrowFireSkill = false;
        FireEffect.SetActive(false);
    }

    public void ShootFireArrow()
    {
        if( CurrentArrowFireSkill && !_isAttacking)
        {
            _playerAnimator.SetTrigger("FireArrowAttack");
            _isAttacking = true;
        }
    }

    public void FireSingleArrow()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        Vector3 shootDirection = (targetPoint - _bowArrowSpawnPoint.position).normalized;

        float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier;

        PlayerArrow arrowInstance = Instantiate(_arrowPrefab, _bowArrowSpawnPoint.position, Quaternion.LookRotation(shootDirection)).GetComponent<PlayerArrow>();
        arrowInstance.ArrowInit(power,ArrowType.Explosive,_player.gameObject);
        arrowInstance.transform.Rotate(_arrowModelRotationOffset, Space.Self);

        Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            StartCoroutine(ApplyForceToArrowAfterDelay(rb, shootDirection.normalized * _arrowForce, _arrowLifetime));
        }
    }

    private IEnumerator ApplyForceToArrowAfterDelay(Rigidbody rb, Vector3 force, float lifetime)
    {
        yield return new WaitForFixedUpdate();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        _isAttacking = false;
    }

    public override void OnSkillAnimationEnd()
    {
        _isAttacking = false;
        _player.CharacterController.stepOffset = 1f;
    }

    public override void Tick()
    {
        if (!CurrentArrowFireSkill) return;

        // 이건 스킬 안에서만 처리하는 Mouse0 입력
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!_isAttacking)
            {
                Debug.Log("FireARROW");
                ShootFireArrow();
            }
        }
    }

    public override void OnSkillEffectPlay() { }

    public override void TryDamageEnemy(GameObject enemy, Vector3 hitDirection)
    {
        //if (!IsUsingSkill) return;

        //float power = _equipmentController.GetCurrentWeaponAttackPower() * _skillDamageMultiplier;

        //IDamageAble damageAble = enemy.GetComponent<IDamageAble>();
        //if (damageAble != null)
        //{
        //    Damage damage = new Damage(power, gameObject, 100f, hitDirection);
        //    damageAble.TakeDamage(damage);
        //    //Debug.Log($"불 화살 스킬로 {enemy.name}에게 {power} 데미지를 입힘!");
        //}
    }
}
