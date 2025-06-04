using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayer : MonoBehaviour, IDamageAble
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;
    private float _currentHealth;
    public float CurrentStamina { get; private set; }


    [Header("참조")]
    public GameObject ReturnVfx;


    private float _gravity = -9.81f;
    private Vector3 _velocity;
    private CharacterController _characterController;
    public CharacterController CharacterController => _characterController;
    private Animator _playerAnimator;
    public Animator PlayerAnimator => _playerAnimator;


    public bool IsAlive = true;
    private float _respawnTime;
    private float _timer;

    public bool IsReturning = false;
    private float _returnTime = 3f;
    private float _returnTimer = 0f;
    private Vector3 _basePosition = new Vector3(400.5f, 15f, 400.5f);

    private float _returnCooldown = 30f;       // 쿨타임 길이
    private float _returnCooldownTimer = 0f;   // 현재 쿨타임 경과 시간
    private bool _isReturnCooldown = false;    // 쿨타임 중 여부


    private float _hitCooldown = 0.2f;     // 피격 쿨타임
    private float _lastHitTime = -999f;   // 마지막 피격 시간


    // 플레이어 버프
    public float BuffSpeed;
    public float BuffDefense;
    public float BuffDamage;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();
        if (PlayerStats)
        {
            _currentHealth = PlayerStats.MaxHealth;
            CurrentStamina = PlayerStats.Stamina;
        }

        IsAlive = true;
        _respawnTime = 4f;
        _timer = 0f;
    }
    private void Start()
    {
        UIManager.instance.UI_PlayerSetMaxStat(PlayerStats.MaxHealth, PlayerStats.Stamina);
    }
    private void Respawn()
    {
        IsAlive = true;
        _timer = 0;
        _characterController.enabled = false;
        gameObject.transform.position = _basePosition;
        _characterController.enabled = true; ;
        _currentHealth = PlayerStats.MaxHealth;

        PlayerAnimator.SetTrigger("Idle");
        UIManager.instance.RespawnPanel.SetActive(false);
        UIManager.instance.UI_HpSlider(_currentHealth);
    }
    private void Update()
    {

        if (!IsAlive)
        {
            _timer += Time.deltaTime;
            UIManager.instance.UI_PlayerRespawn(_timer, _respawnTime);
            if (_timer >= _respawnTime)
            {
                Respawn();
            }
            return;
        }

        // 귀환 타이머 진행
        if (IsReturning)
        {
            _returnTimer += Time.deltaTime;
            UIManager.instance.UI_PlayerRespawn(_returnTimer, _returnTime);

            if (_returnTimer >= _returnTime)
            {
                CompleteReturnToBase();
            }
        }

        // 귀환 쿨타임 진행
        if (_isReturnCooldown)
        {
            _returnCooldownTimer += Time.deltaTime;

            if (_returnCooldownTimer >= _returnCooldown)
            {
                _isReturnCooldown = false;
                _returnCooldownTimer = 0f;
                Debug.Log("귀환 재사용 가능");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UsePosion();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReturnToBase();
        }
    }

    private void DieSet()
    {
        IsAlive = false;
        // 애니메이션 이동 파라미터 초기화
        PlayerAnimator.SetFloat("MoveX", 0);
        PlayerAnimator.SetFloat("MoveY", 0);
        PlayerAnimator.SetFloat("MoveSpeed", 0);
        PlayerAnimator.SetTrigger("Die");
    }

    private void StartReturnToBase()
    {
        if (IsReturning || !IsAlive || _isReturnCooldown) return;


        IsReturning = true;
        _returnTimer = 0f;
        UIManager.instance.RespawnPanel.SetActive(true); 

        ReturnVfx.SetActive(true);
        PlayerAnimator.SetTrigger("Idle");
        // 애니메이션 이동 파라미터 초기화
        PlayerAnimator.SetFloat("MoveX", 0);
        PlayerAnimator.SetFloat("MoveY", 0);
        PlayerAnimator.SetFloat("MoveSpeed", 0);
        Debug.Log("귀환 시작!");
    }

    private void CompleteReturnToBase()
    {
        IsReturning = false;
        _returnTimer = 0f;

        _characterController.enabled = false;
        transform.position = _basePosition;
        _characterController.enabled = true;

        UIManager.instance.RespawnPanel.SetActive(false); // 귀환 UI 숨기기

        ReturnVfx.SetActive(false);

        _isReturnCooldown = true;
        _returnCooldownTimer = 0f;
        Debug.Log("귀환 완료!");
    }
    private void UsePosion()
    {
        if(PlayerRewardManager.Instance.UsePotion() && _currentHealth< PlayerStats.MaxHealth)
        {
            float pluseHealth = PlayerStats.MaxHealth / 2;

            _currentHealth = Mathf.Clamp(_currentHealth + pluseHealth, 0, PlayerStats.MaxHealth);

            UIManager.instance.UI_HpSlider(_currentHealth);
            PlayerSoundController.Instance.PlaySound(PlayerSoundType.PosionSound);
        }
    }

    public void TakeDamage(Damage damage)
    {
        if (!IsAlive) return;

        if (Time.time - _lastHitTime < _hitCooldown)
            return;

        _lastHitTime = Time.time;

        // 방어력 버프 적용
        float reducedDamage = damage.Value - BuffDefense;
        reducedDamage = Mathf.Max(reducedDamage, 1f); // 최소 데미지

        _currentHealth -= reducedDamage;
        UIManager.instance.UI_HpSlider(_currentHealth);

        CameraShakeManager.Instance.Shake(0.1f, 0.05f);
        _playerAnimator.SetTrigger("Hit");
        PlayerSoundController.Instance.PlaySound(PlayerSoundType.Hit);

        if (_currentHealth <= 0)
        {
            DieSet();
        }
    }

    public void UseStamina(float amount)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina -  amount, 0f, PlayerStats.Stamina);
        UIManager.instance.UI_MpSlider(CurrentStamina);
    }

    public void ReduceStamina(float amount)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina - amount, 0f, PlayerStats.Stamina);
        UIManager.instance.UI_MpSlider(CurrentStamina);
    }

    public void RecoverStamina()
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina + Time.deltaTime * 5, 0f, PlayerStats.Stamina);
        UIManager.instance.UI_MpSlider(CurrentStamina);
    }
    public void RecoveryStamina(float value)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina + value, 0f, PlayerStats.Stamina);
        UIManager.instance.UI_MpSlider(CurrentStamina);
    }
}
