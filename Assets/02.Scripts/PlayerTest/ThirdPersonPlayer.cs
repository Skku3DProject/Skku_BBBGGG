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
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();
        if (PlayerStats)
        {
            _currentHealth = PlayerStats.MaxHealth;
            CurrentStamina = PlayerStats.Stamina;
        }
      
    }
    private void Start()
    {
            UIManager.instance.UI_PlayerSetMaxStat(PlayerStats.MaxHealth, PlayerStats.Stamina);
    }
    private void Update()
    {

    }
    public void TakeDamage(Damage damage)
    {
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
