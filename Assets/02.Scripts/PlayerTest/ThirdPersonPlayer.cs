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
        Debug.Log(gameObject.transform.position);
        UIManager.instance.RespawnPanel.SetActive(false);
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
    private void StartReturnToBase()
    {
        if (IsReturning || !IsAlive || _isReturnCooldown) return;


        IsReturning = true;
        _returnTimer = 0f;
        UIManager.instance.RespawnPanel.SetActive(true); 

        ReturnVfx.SetActive(true);
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
        }
    }

    public void TakeDamage(Damage damage)
    {
        _currentHealth -= damage.Value;
        UIManager.instance.UI_HpSlider(_currentHealth);

        if (_currentHealth <= 0)
        {
            IsAlive = false;
            Debug.Log(" DIE");
        }
    }

    public void UseStamina(float amount)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina - Time.deltaTime * amount, 0f, PlayerStats.Stamina);
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
}
