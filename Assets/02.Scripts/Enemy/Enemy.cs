using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySo EnemyData;

    private GameObject _target;
    public GameObject Target => _target;

    private GameObject _gaol; // 베이스 캠프
    public GameObject Gaol => _gaol;

    private GameObject _player;
    public GameObject Player => _player;

    public Vector3 GravityVelocity;

    public List<GameObject> others;

    public CharacterController CharacterController;
    public Animator Animator;

    private float _maxHealth;
    private float _health;
    public float Health => _health;

    private const int MaxHits = 8;
    private readonly Collider[] _hits = new Collider[MaxHits];

    [Header("UI")]
    public UI_EnemyHpbar UI_EnemyHpbar;
    public Transform UI_offset;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
       
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
        _gaol = GameObject.FindGameObjectWithTag("BaseTower");
        _player = GameObject.FindGameObjectWithTag("Player");

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
        if (Vector3.Distance(transform.position, Player.transform.position) < EnemyData.AttackDistance)
        {
            return true;
        }
        return false;
    }
    public bool FindTarget()
    {
        Vector3 origin = transform.position;
        float radius = EnemyData.FindDistance;

        // NonAlloc 호출: _hits 배열에 충돌체 결과를 채우고, 갯수를 반환
        int hitCount = Physics.OverlapSphereNonAlloc(
            origin,
            radius,
            _hits,
            EnemyData.playerLayerMask,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider collider = _hits[i];
            if (collider != null && collider.CompareTag("Player"))
            {
                OnPlayerDetected(collider.gameObject);
                return true;
            }
        }
       // OnPlayerLost();
        return false;
    }

    private void OnPlayerDetected(GameObject player)
    {
        _target = player;
    }

    private void OnPlayerLost()
    {
        _target = _gaol;
    }

}
