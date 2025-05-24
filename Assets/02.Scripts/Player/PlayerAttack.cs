using Unity.Cinemachine;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;

    public bool IsAttacking {  get; private set; }
    private int _comboStep = 0;
    private bool _canNextCombo = true;
    private bool _nextComboQueued = false;
    private float _comboResetTime = 1.0f; // 콤보 유지 시간 (초)
    private float _lastAttackTime;
    [Header("Attack Effects")]
    public ParticleSystem SwordSlashEffect;


    // [Header("Attack Settings")]
    //public PlayerStatsSO PlayerStats;

    private int _currentAttackIndex; // 현재 공격 인덱스 기억용

    void Start()
    {
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();

        PlayerModeManager.OnModeChanged += OnModeChanged;

    }

    void Update()
    {
        if (PlayerModeManager.Instance.CurrentMode != EPlayerMode.Weapon) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsAttacking)
            {
                _comboStep = 0;
                IsAttacking = true;
                PlayComboAttack(_comboStep);
                _lastAttackTime = Time.time;
            }
            else if (_canNextCombo)
            {
                _nextComboQueued = true;
                _lastAttackTime = Time.time;
            }
        }

        // 콤보 시간 초과 초기화
        if (IsAttacking && Time.time - _lastAttackTime > _comboResetTime)
        {
            ResetCombo();
        }
    }
    private void PlayComboAttack(int index)
    {
        _playerAnimation.SetFloat("AttackIndex", index);
        _playerAnimation.SetTrigger("Attack");

        _currentAttackIndex = index;
        _canNextCombo = false;
        _nextComboQueued = false;
    }
    private void ResetCombo()
    {
        IsAttacking = false;
        _comboStep = 0;
        _playerAnimation.SetFloat("AttackIndex", 0);
        _canNextCombo = false;
        _nextComboQueued = false;
    }
    //검이 적과 충돌
    public void TryDamageEnemy(GameObject enemy, Vector3 dir)
    {
        // Debug.Log("적과 충돌해서 공격할거임");

        if (!IsAttacking)
        {
            return;
        }
        // 현재 무기 타입 확인
        var currentEquipType = _equipmentController.GetCurrentEquipType();
        //Debug.Log($"현재 장착 무기 타입: {currentEquipType}");

        // 공격력 확인
        float attackPower = _equipmentController.GetCurrentWeaponAttackPower();
        // Debug.Log($"현재 무기 공격력: {attackPower}");

        // 데미지 줄 수 있는 대상인지 확인
        IDamageAble damageable = enemy.GetComponent<IDamageAble>();
        if (damageable != null)
        {
            Damage damage = new Damage(attackPower, gameObject, 10f, dir);
            damageable.TakeDamage(damage);
            Debug.Log($"공격 성공: {enemy.name}에게 {attackPower} 데미지를 줌");
        }
        else
        {
            Debug.LogWarning($"IDamageAble 컴포넌트를 {enemy.name}에서 찾을 수 없음");
        }
    }

    // 애니메이션 이벤트에서 호출됨
    public void OnAttackEffectPlay()=> SwordSlashEffect.Play();
    

    // 애니메이션 이벤트에서 호출됨 (마지막 프레임 근처)
    public void OnAttackAnimationEnd()
    {
        Debug.Log("??");

        if (_nextComboQueued)
        {
            _comboStep++;
            if (_comboStep > 3) // 최대 콤보 단계 제한 (0~4 총 5단계)
            {
                ResetCombo();
                return;
            }
            PlayComboAttack(_comboStep);
            _lastAttackTime = Time.time;
        }
        else
        {
            ResetCombo();
        }
    }
    private void OnModeChanged(EPlayerMode type)
    {
        if (type != EPlayerMode.Weapon) return;

        IsAttacking = false;
    }
    public void EnableComboInput()
    {
        _canNextCombo = true;
    }
}
