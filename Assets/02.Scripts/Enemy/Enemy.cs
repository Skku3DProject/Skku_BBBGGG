using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public So_Enemy EnemyData;

    public Transform ProjectileTransfrom;

    private GameObject _target;
    public GameObject Target => _target;

    private GameObject _gaol;
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

    private float _stepOffset = 0;
    public float StepOffset => _stepOffset;

    private void Awake()
    {
        _gaol = GameObject.FindGameObjectWithTag("BaseTower");
        _player = GameObject.FindGameObjectWithTag("Player");
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _findDistance = EnemyData.FindDistance * EnemyData.FindDistance;
        _attackDistance = EnemyData.AttackDistance * EnemyData.AttackDistance;
        _stepOffset = _characterController.stepOffset;
    }
    
    public void Initialize()
    {
        _target = _gaol;
        _health = EnemyData.Health;
        _maxHealth = _health;

        if (UI_EnemyHpbar != null)
        {
            UI_EnemyHpbar.Initialized();
        }

        EnemyManager.Instance.SetMoveTypeGrouping(this);
        gameObject.SetActive(true);
	}

    public void TakeDamage(Damage damage)
    {
        _health -= damage.Value;
        if (damage.From.CompareTag("Player"))
        {
            GoOnPlayer();
        }
        else
        {
            _target = damage.From.gameObject;
        }

        UI_EnemyHpbar.UpdateHealth(_health / _maxHealth);
    }

    public bool TryAttack()
    {
        if(_target.gameObject.activeInHierarchy == false && _target != _gaol)
        {
            _target = _gaol;
        }

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
            GoOnPlayer();
            return;
        }
    }

    private void GoOnPlayer()
    {
        EnemyManager.Instance.SetTargetGrouping(this);
        _target = _player;
    }
  

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        float radius = Mathf.Sqrt(EnemyData.FindDistance);

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
