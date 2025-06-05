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

    // 상태용 필드
    private float _currentHP;
    private float _cooldownTimer = 0f;
    private Animator _animator;
    private CharacterController _controller;
    private GameObject _target;
    private MeleeUnitStateType _currentState;
    private bool _isAttacking = false; // 공격 애니메이션 중인지 여부

    private Vector3 _savedPosition;
    private Quaternion _savedRotation;

    // 중력 처리용
    private float _verticalVelocity = 0f;
    // 수평 이동 방향 (XZ 면)
    private Vector3 _moveDirection = Vector3.zero;

    // 타깃 갱신용 타이머 (공격 중 매 0.5초마다 갱신)
    private float _targetRefreshTimer = 0f;
    private const float TargetRefreshInterval = 0.5f;

    private void Awake()    
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _currentHP = MaxHP;
        // 초기 상태를 Idle로 설정
        _currentState = MeleeUnitStateType.Idle;
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetBool("Idle", true);
            _animator.SetBool("Move", false);
        }
    }
    private void OnEnable()
    {
        // 체력 초기화
        _currentHP = MaxHP;

        // 상태 초기화
        _currentState = MeleeUnitStateType.Idle;
        _cooldownTimer = AttackCooldown;
        _verticalVelocity = -1f;
        _moveDirection = Vector3.zero;
        _target = null;
        _isAttacking = false;

        // 애니메이터 초기화
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.Rebind(); // 모든 트리거, 상태 초기화
            _animator.Update(0f); // 즉시 반영
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
        // 1) 쿨다운 타이머 갱신
        if (_cooldownTimer < AttackCooldown)
            _cooldownTimer += Time.deltaTime;

        // 2) 상태 머신
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

                // Dead는 애니메이션/풀 반환 로직으로 처리됨
        }

        // 3) 중력 처리
        if (_controller.isGrounded)
            _verticalVelocity = -1f;
        else
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;

        // 4) 최종 이동벡터 = (XZ 이동) + (Y 중력)
        Vector3 finalVelocity = _moveDirection + Vector3.up * _verticalVelocity;
        // 5) CharacterController로 실제 이동
        _controller.Move(finalVelocity * Time.deltaTime);
    }

    private void ChangeState(MeleeUnitStateType newState)
    {
        // ★ 새로운 상태가 현재 상태와 같으면 아무것도 하지 않음
        if (newState == _currentState)
            return;

        // Animator에 Idle/Move Bool 파라미터만 세팅
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetBool("Idle", newState == MeleeUnitStateType.Idle);
            _animator.SetBool("Move", newState == MeleeUnitStateType.Move);
        }

        _currentState = newState;
        // 상태 전환 시 수평 이동 방향 초기화
        _moveDirection = Vector3.zero;

        // 공격 상태로 진입할 때만 타이머 초기화
        if (newState == MeleeUnitStateType.Attack)
            _targetRefreshTimer = TargetRefreshInterval;
    }

    private void SearchForTarget()
    {
        // 기존 타겟이 비활성화되었거나 Fly 타입이면 해제
        if (_target != null)
        {
            if (!_target.activeInHierarchy ||
                (_target.TryGetComponent(out Enemy oldEnemy) && oldEnemy.EnemyData.EnemyMoveType == EEnemyMoveType.Fly))
            {
                _target = null;
            }
        }

        // 타겟이 없을 때만 탐색 시도
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

        // Idle 상태이므로 이동은 0
        _moveDirection = Vector3.zero;
    }

    private void MoveToTarget()
    {
        // _target이 없거나 비활성화되었거나 Fly 타입이면 Idle로 복귀
        if (_target == null ||
            !_target.activeInHierarchy ||
            (_target.TryGetComponent(out Enemy e) && e.EnemyData.EnemyMoveType == EEnemyMoveType.Fly))
        {
            _target = null;
            ChangeState(MeleeUnitStateType.Idle);
            return;
        }

        // 타겟과의 수평 거리 계산
        Vector3 dir = _target.transform.position - transform.position;
        dir.y = 0f;
        float distance = dir.magnitude;

        // 사거리 안이면 Attack으로 전환
        if (distance <= AttackRange)
        {
            ChangeState(MeleeUnitStateType.Attack);
            _isAttacking = false; // 애니메이션 시작 전
            _moveDirection = Vector3.zero;
            return;
        }

        // 사거리 밖이면 계속 이동
        Vector3 moveDir = dir.normalized;
        _moveDirection = moveDir * MoveSpeed;

        // 바라보는 방향만 회전
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Vector3 lookDir = moveDir;
            lookDir.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    private void AttackUpdate()
    {
        // 이미 공격 애니메이션 중이라면 이동만 막고 리턴
        if (_isAttacking)
        {
            _moveDirection = Vector3.zero;
            return;
        }

        // _target이 없거나 비활성화되었거나 Fly 타입이면 Idle로
        if (_target == null ||
            !_target.activeInHierarchy ||
            (_target.TryGetComponent(out Enemy e) && e.EnemyData.EnemyMoveType == EEnemyMoveType.Fly))
        {
            _target = null;
            ChangeState(MeleeUnitStateType.Idle);
            return;
        }

        // 사거리 체크
        float dist = Vector3.Distance(transform.position, _target.transform.position);
        if (dist > AttackRange)
        {
            ChangeState(MeleeUnitStateType.Move);
            return;
        }

        // 사거리 안, 공격 준비가 되면 Attack 트리거
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

        // 일단 공격 애니메이션이 시작되기 전까지(_isAttacking == false), 타이머마다 타겟 갱신
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

    // “AttackHit” 이벤트: 애니메이션에서 데미지 판정 프레임에 호출
    public void AttackHit()
    {
        // _target이 없거나 비활성화되었거나 Fly 타입이면 리턴
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

    // “AttackEnd” 이벤트: 공격 애니메이션이 끝난 프레임에 호출
    public void AttackEnd()
    {
        _isAttacking = false;

        // _target이 없거나 비활성화되었거나 Fly 타입이면 Idle
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
    // 피해 입는 함수 & 사망 처리
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
    // Helper: 공격 중 타깃을 갱신하는 함수
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