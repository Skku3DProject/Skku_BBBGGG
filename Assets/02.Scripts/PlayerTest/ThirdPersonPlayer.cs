using UnityEngine;

public class ThirdPersonPlayer : MonoBehaviour, IDamageAble
{
    [Header("Player Stats")]
    public PlayerStatsSO PlayerStats;
    private float _currentHealth;

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
            _currentHealth = PlayerStats.MaxHealth;
    }
    public void TakeDamage(Damage damage)
    {
    }
}
