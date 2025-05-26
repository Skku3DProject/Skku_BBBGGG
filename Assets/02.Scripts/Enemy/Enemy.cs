using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public So_Enemy EnemyData;

    public Transform ProjectileTransfrom;

    private GameObject _target;
    public GameObject Target => _target;

    private GameObject _gaol; // ���̽� ķ��
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
    
    void OnEnable()
    {
        EnemyManager.Instance.Register(this);
        if(UI_EnemyHpbar != null)
        {
            UI_EnemyHpbar.Initialized();
        }
    }

    void OnDisable()
    {
        EnemyManager.Instance.Unregister(this);
    }
    
    public void Initialize()
    {
        _target = _gaol;
        _health = EnemyData.Health;
        _maxHealth = _health;
		gameObject.SetActive(true);
		UI_EnemyHpbar.Initialized();
	}

    public void TakeDamage(Damage damage)
    {
        _health -= damage.Value;
        UI_EnemyHpbar.UpdateHealth(_health / _maxHealth);
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
    public void OnPlayer()
    {
        _target = _player;
    }
    private void OnPlayerLost()
    {
        _target = _gaol;
    }

    private void OnDrawGizmosSelected()
    {
        // Ž�� �Ÿ� �ð�ȭ (�Ķ���)
        Gizmos.color = Color.cyan;

        // _findDistance�� SqrMagnitude �����̹Ƿ� ��Ʈ�� ������ ���� �Ÿ�
        float radius = Mathf.Sqrt(EnemyData.FindDistance);

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
