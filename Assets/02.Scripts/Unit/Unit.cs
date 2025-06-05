using System.Collections.Generic;
using UnityEngine;
public enum MeleeUnitStateType
{
    Idle,
    Move,
    Attack,
    Dead
}
public class Unit : MonoBehaviour
{
    [Header("Unit Stats")]
    public float MaxHP = 100f;
    public float MoveSpeed = 3f;
    public float AttackRange = 2f;
    [Range(0f, 360f)] public float AttackAngle = 90f;
    public float AttackCooldown = 1f;
    public float DetectRange = 10f;
    public LayerMask EnemyLayer;

    // ���¿� �ʵ�
    private float _currentHP;
    private float _cooldownTimer = 0f;
    private Animator _animator;
    private CharacterController _controller;
    private GameObject _target;
    private MeleeUnitStateType _currentState;
    private bool _isAttacking = false; // ���� �ִϸ��̼� ������ ����

    private Vector3 _savedPosition;
    private Quaternion _savedRotation;

    // �߷� ó����
    private float _verticalVelocity = 0f;
    // ���� �̵� ���� (XZ ��)
    private Vector3 _moveDirection = Vector3.zero;

    // Ÿ�� ���ſ� Ÿ�̸� (���� �� �� 0.5�ʸ��� ����)
    private float _targetRefreshTimer = 0f;
    private const float TargetRefreshInterval = 0.5f;

    private void Awake()    
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _currentHP = MaxHP;
        // �ʱ� ���¸� Idle�� ����
        _currentState = MeleeUnitStateType.Idle;
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetBool("Idle", true);
            _animator.SetBool("Move", false);
        }
    }
    private void OnEnable()
    {
        // ü�� �ʱ�ȭ
        _currentHP = MaxHP;

        // ���� �ʱ�ȭ
        _currentState = MeleeUnitStateType.Idle;
        _cooldownTimer = AttackCooldown;
        _verticalVelocity = -1f;
        _moveDirection = Vector3.zero;
        _target = null;
        _isAttacking = false;

        // �ִϸ����� �ʱ�ȭ
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.Rebind(); // ��� Ʈ����, ���� �ʱ�ȭ
            _animator.Update(0f); // ��� �ݿ�
            _animator.SetBool("Idle", true);
            _animator.SetBool("Move", false);
        }
    }
    private void Start()
    {
        StageManager.instance.OnCombatStart += HandleCombatStart;
        StageManager.instance.OnCombatEnd += HandleCombatEnd;
    }
    private void Update()
    {
        // 1) ��ٿ� Ÿ�̸� ����
        if (_cooldownTimer < AttackCooldown)
            _cooldownTimer += Time.deltaTime;

        // 2) ���� �ӽ�
        switch (_currentState)
        {
            case MeleeUnitStateType.Idle:
                SearchForTarget();
                break;

            case MeleeUnitStateType.Move:
                MoveToTarget();
                break;

            case MeleeUnitStateType.Attack:
                AttackUpdate();
                break;

                // Dead�� �ִϸ��̼�/Ǯ ��ȯ �������� ó����
        }

        // 3) �߷� ó��
        if (_controller.isGrounded)
            _verticalVelocity = -1f;
        else
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;

        // 4) ���� �̵����� = (XZ �̵�) + (Y �߷�)
        Vector3 finalVelocity = _moveDirection + Vector3.up * _verticalVelocity;
        // 5) CharacterController�� ���� �̵�
        _controller.Move(finalVelocity * Time.deltaTime);
    }

    private void ChangeState(MeleeUnitStateType newState)
    {
        // �� ���ο� ���°� ���� ���¿� ������ �ƹ��͵� ���� ����
        if (newState == _currentState)
            return;

        // Animator�� Idle/Move Bool �Ķ���͸� ����
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetBool("Idle", newState == MeleeUnitStateType.Idle);
            _animator.SetBool("Move", newState == MeleeUnitStateType.Move);
        }

        _currentState = newState;
        // ���� ��ȯ �� ���� �̵� ���� �ʱ�ȭ
        _moveDirection = Vector3.zero;

        // ���� ���·� ������ ���� Ÿ�̸� �ʱ�ȭ
        if (newState == MeleeUnitStateType.Attack)
            _targetRefreshTimer = TargetRefreshInterval;
    }

    private void SearchForTarget()
    {
        // ���� Ÿ���� ��Ȱ��ȭ�Ǿ��ų� Fly Ÿ���̸� ����
        if (_target != null)
        {
            if (!_target.activeInHierarchy ||
                (_target.TryGetComponent(out Enemy oldEnemy) && oldEnemy.EnemyData.EnemyMoveType == EEnemyMoveType.Fly))
            {
                _target = null;
            }
        }

        // Ÿ���� ���� ���� Ž�� �õ�
        if (_target == null)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, DetectRange, EnemyLayer);
            GameObject candidate = null;
            float minDist = float.MaxValue;

            foreach (var hit in hits)
            {
                if (!hit.gameObject.activeInHierarchy)
                    continue;

                if (hit.TryGetComponent(out Enemy e) && e.EnemyData.EnemyMoveType != EEnemyMoveType.Fly)
                {
                    float d = Vector3.Distance(transform.position, hit.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        candidate = hit.gameObject;
                    }
                }
            }

            if (candidate != null)
            {
                _target = candidate;
                ChangeState(MeleeUnitStateType.Move);
                return;
            }
        }

        // Idle �����̹Ƿ� �̵��� 0
        _moveDirection = Vector3.zero;
    }

    private void MoveToTarget()
    {
        // _target�� ���ų� ��Ȱ��ȭ�Ǿ��ų� Fly Ÿ���̸� Idle�� ����
        if (_target == null ||
            !_target.activeInHierarchy ||
            (_target.TryGetComponent(out Enemy e) && e.EnemyData.EnemyMoveType == EEnemyMoveType.Fly))
        {
            _target = null;
            ChangeState(MeleeUnitStateType.Idle);
            return;
        }

        // Ÿ�ٰ��� ���� �Ÿ� ���
        Vector3 dir = _target.transform.position - transform.position;
        dir.y = 0f;
        float distance = dir.magnitude;

        // ��Ÿ� ���̸� Attack���� ��ȯ
        if (distance <= AttackRange)
        {
            ChangeState(MeleeUnitStateType.Attack);
            _isAttacking = false; // �ִϸ��̼� ���� ��
            _moveDirection = Vector3.zero;
            return;
        }

        // ��Ÿ� ���̸� ��� �̵�
        Vector3 moveDir = dir.normalized;
        _moveDirection = moveDir * MoveSpeed;

        // �ٶ󺸴� ���⸸ ȸ��
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Vector3 lookDir = moveDir;
            lookDir.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    private void AttackUpdate()
    {
        // �̹� ���� �ִϸ��̼� ���̶�� �̵��� ���� ����
        if (_isAttacking)
        {
            _moveDirection = Vector3.zero;
            return;
        }

        // _target�� ���ų� ��Ȱ��ȭ�Ǿ��ų� Fly Ÿ���̸� Idle��
        if (_target == null ||
            !_target.activeInHierarchy ||
            (_target.TryGetComponent(out Enemy e) && e.EnemyData.EnemyMoveType == EEnemyMoveType.Fly))
        {
            _target = null;
            ChangeState(MeleeUnitStateType.Idle);
            return;
        }

        // ��Ÿ� üũ
        float dist = Vector3.Distance(transform.position, _target.transform.position);
        if (dist > AttackRange)
        {
            ChangeState(MeleeUnitStateType.Move);
            return;
        }

        // ��Ÿ� ��, ���� �غ� �Ǹ� Attack Ʈ����
        Vector3 lookDir = _target.transform.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(lookDir.normalized);

        if (_cooldownTimer >= AttackCooldown)
        {
            _cooldownTimer = 0f;
            _isAttacking = true;
            _animator.SetTrigger("Attack");
            return;
        }

        // �ϴ� ���� �ִϸ��̼��� ���۵Ǳ� ������(_isAttacking == false), Ÿ�̸Ӹ��� Ÿ�� ����
        _targetRefreshTimer -= Time.deltaTime;
        if (_targetRefreshTimer <= 0f)
        {
            RefreshAttackTarget();
            _targetRefreshTimer = TargetRefreshInterval;
        }
    }

    // =========================
    // Animation Event Methods
    // =========================

    // ��AttackHit�� �̺�Ʈ: �ִϸ��̼ǿ��� ������ ���� �����ӿ� ȣ��
    public void AttackHit()
    {
        // _target�� ���ų� ��Ȱ��ȭ�Ǿ��ų� Fly Ÿ���̸� ����
        if (_target == null ||
            !_target.activeInHierarchy ||
            (_target.TryGetComponent(out Enemy e) && e.EnemyData.EnemyMoveType == EEnemyMoveType.Fly))
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, AttackRange, EnemyLayer);
        GameObject nearestTarget = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.gameObject.activeInHierarchy)
                continue;

            if (hit.TryGetComponent(out Enemy en) && en.EnemyData.EnemyMoveType != EEnemyMoveType.Fly)
            {
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);

                if (angle <= AttackAngle / 2f)
                {
                    if (en.TryGetComponent(out IDamageAble targetComp))
                    {
                        targetComp.TakeDamage(new Damage(50f, gameObject, 5f, dirToTarget));
                    }

                    float d = Vector3.Distance(transform.position, hit.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        nearestTarget = hit.gameObject;
                    }
                }
            }
        }

        if (nearestTarget != null)
            _target = nearestTarget;
    }

    // ��AttackEnd�� �̺�Ʈ: ���� �ִϸ��̼��� ���� �����ӿ� ȣ��
    public void AttackEnd()
    {
        _isAttacking = false;

        // _target�� ���ų� ��Ȱ��ȭ�Ǿ��ų� Fly Ÿ���̸� Idle
        if (_target == null ||
            !_target.activeInHierarchy ||
            (_target.TryGetComponent(out Enemy e) && e.EnemyData.EnemyMoveType == EEnemyMoveType.Fly))
        {
            _target = null;
            ChangeState(MeleeUnitStateType.Idle);
            return;
        }

        float dist = Vector3.Distance(transform.position, _target.transform.position);
        if (dist > AttackRange)
            ChangeState(MeleeUnitStateType.Move);
        else
            ChangeState(MeleeUnitStateType.Attack);
    }

    // =========================
    // ���� �Դ� �Լ� & ��� ó��
    // =========================
    public void TakeDamage(Damage damage)
    {
        _currentHP -= damage.Value;
        if (_currentHP <= 0f && _currentState != MeleeUnitStateType.Dead)
        {
            Die();
        }
    }

    private void Die()
    {
        ChangeState(MeleeUnitStateType.Dead);
        _animator.SetTrigger("Die");
        ObjectPool.Instance.ReturnToPool(gameObject);
    }

    // =========================
    // Helper: ���� �� Ÿ���� �����ϴ� �Լ�
    // =========================
    private void RefreshAttackTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, AttackRange, EnemyLayer);
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.gameObject.activeInHierarchy)
                continue;

            if (hit.TryGetComponent(out Enemy en) && en.EnemyData.EnemyMoveType != EEnemyMoveType.Fly)
            {
                Vector3 dirTo = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirTo);
                if (angle > AttackAngle / 2f)
                    continue;

                float d = Vector3.Distance(transform.position, hit.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = hit.gameObject;
                }
            }
        }

        if (nearest != null)
            _target = nearest;
    }

    private void HandleCombatStart()
    {
        _savedPosition = transform.position;
        _savedRotation = transform.rotation;
    }

    private void HandleCombatEnd()
    {
        if (TryGetComponent(out CharacterController controller))
        {
            controller.enabled = false;
            transform.position = _savedPosition;
            transform.rotation = _savedRotation;
            controller.enabled = true;
        }
        else
        {
            transform.position = _savedPosition;
            transform.rotation = _savedRotation;
        }
    }
}