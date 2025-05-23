using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public So_Enemy EnemyData;

    private GameObject _target;
    public GameObject Target => _target;

    private GameObject _gaol; // º£ÀÌ½º Ä·ÇÁ
    public GameObject Gaol => _gaol;

    private GameObject _player;
    public GameObject Player => _player;

    public Vector3 GravityVelocity;

    private CharacterController _characterController;
    public CharacterController CharacterController => _characterController;

    private Animator _animator;
    public Animator Animator => _animator;

    private float _maxHealth;
    private float _health;
    public float Health => _health;

    private float _findDistance;
    private float _attackDistance;

    public Vector3 CurrentMoveDirection;

    [Header("UI")]
    public UI_EnemyHpbar UI_EnemyHpbar;
    public Transform UI_offset;

    private void Awake()
    {
        _gaol = GameObject.FindGameObjectWithTag("BaseTower");
        _player = GameObject.FindGameObjectWithTag("Player");
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _findDistance = EnemyData.FindDistance * EnemyData.FindDistance;
        _attackDistance = EnemyData.AttackDistance * EnemyData.AttackDistance;
    }
    
    void OnEnable()
    {
        EnemyManager.Instance.Register(this);
        if(UI_EnemyHpbar != null)
        {
            UI_EnemyHpbar.Initialized();
        }
        EnemyManager.Instance.Enable(this);
    }

    void OnDisable()
    {
        EnemyManager.Instance.Unregister(this);
        EnemyManager.Instance.UnEnable(this);
    }
    
    public void Initialize()
    {
        _target = _gaol;
        _health = EnemyData.Health;
        _maxHealth = _health;
    }

    public void TakeDamage(Damage damage)
    {
        _health -= damage.Value;
        UI_EnemyHpbar.UpdateHealth(_health / _maxHealth);
        Debug.Log($"TakeDamageEnemy HP: {_health}");
    }

    public bool TryAttack()
    {
        if ((transform.position - _target.transform.position).sqrMagnitude < _attackDistance)
        {
            return true;
        }
        return false;
    }

    public void TargetOnPlayer()
    {
        if(_target == _player)
        {
            return;
        }
        if ((transform.position - Player.transform.position).sqrMagnitude < _findDistance)
        {
            EnemyManager.Instance.Unregister(this);
            _target = _player;
            return;
        }
    }

    private void OnPlayerLost()
    {
        _target = _gaol;
    }

}
