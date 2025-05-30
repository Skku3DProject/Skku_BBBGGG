using UnityEngine;

public class ThirdPersonPlayer : MonoBehaviour, IDamageAble
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;
    private float _currentHealth;
    public float CurrentStamina { get; private set; }


    private float _gravity = -9.81f;
    private Vector3 _velocity;
    private CharacterController _characterController;
    public CharacterController CharacterController => _characterController;
    private Animator _playerAnimator;
    public Animator PlayerAnimator => _playerAnimator;


    public bool IsAlive = true;
    private float _respawnTime;
    private float _timer;


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
        gameObject.transform.position = new Vector3(400.5f, 15f, 400.5f);
        _characterController.enabled = true; ;
        _currentHealth = PlayerStats.MaxHealth;
        Debug.Log(gameObject.transform.position);
        UIManager.instance.RespawnPanel.SetActive(false);
    }
    private void Update()
    {
        if(!IsAlive)
        {
            Debug.Log(" DIE RESPAWN");
            _timer += Time.deltaTime;
            UIManager.instance.UI_PlayerRespawn(_timer, _respawnTime);

            if(_timer >= _respawnTime)
            {
                Respawn();
            }
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
        CurrentStamina = Mathf.Clamp(CurrentStamina + Time.deltaTime *5, 0f, PlayerStats.Stamina);
        UIManager.instance.UI_MpSlider(CurrentStamina);
    }
}
