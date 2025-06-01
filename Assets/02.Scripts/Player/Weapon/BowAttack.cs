using UnityEngine;
using System.Collections;
public enum BowSkillState
{
    None,
    FireArrow,
    TripleArrow,
    Normal
}

public class BowAttack : WeaponAttackBase
{
    private Animator _playerAnimation;
    private ThirdPersonPlayer _player;
    private BowFireSkill _fireSkill;

    [SerializeField] private LayerMask targetLayerMask;

    [Header("화살세팅")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float shootForce = 30f;
    public float Damage = 50f;
    public TrajectoryRenderer trajectoryRenderer;

    private bool isAiming = false;
    private bool _canShootNext = true;
    private float _lastAttackTime;
    private float _comboResetTime = 1.0f;

    void Awake()
    {
        _playerAnimation = GetComponent<Animator>();
        _player = GetComponent<ThirdPersonPlayer>();
        _fireSkill = GetComponent<BowFireSkill>();
    }

    public override void Tick()
    {
        base.Tick();

        // 우클릭 조준 시작
        if (Input.GetMouseButtonDown(1))
        {
            BeginAiming();
        }
        // 우클릭 해제 시 조준 종료
        else if (Input.GetMouseButtonUp(1))
        {
            EndAiming();
        }

        // 공격 상태 초기화
        if (!isAiming && IsAttacking && Time.time - _lastAttackTime > _comboResetTime)
        {
            ResetAttack();
        }

        // 궤적 렌더링
        if (trajectoryRenderer != null)
        {
            if (isAiming)
                DrawTrajectory();
            else
                trajectoryRenderer.ClearTrajectory();
        }
    }

    public override void Attack()
    {
        if (_fireSkill?.CurrentArrowFireSkill == true) return;

        if (!_canShootNext) return;

        if (isAiming)
        {
            ShootArrow();
        }
        else
        {
            _playerAnimation.SetTrigger("Attack");
        }

        IsAttacking = true;
        _canShootNext = false;
        _lastAttackTime = Time.time;

        // 조준 공격이라면 해제 처리
        if (isAiming)
            StartCoroutine(EndAimAfterDelay(0.05f));
    }

    private void BeginAiming()
    {
        isAiming = true;
        IsAttacking = true; // 이동 속도 조절용으로 필요
        _canShootNext = true;

        _player.PlayerAnimator.SetTrigger("Aim");
        _player.PlayerAnimator.SetBool("IsAiming", true);
    }

    private void EndAiming()
    {
        isAiming = false;
        IsAttacking = false;
        _player.PlayerAnimator.SetBool("IsAiming", false);
    }

    private IEnumerator EndAimAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndAiming();
        ResetAttack();
    }

    private void ResetAttack()
    {
        IsAttacking = false;
        _canShootNext = true;
    }

    private void DrawTrajectory()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 target;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, targetLayerMask))
            target = hit.point;
        else
            target = ray.origin + ray.direction * 100f;

        Vector3 direction = (target - shootPoint.position).normalized;
        Vector3 velocity = direction * shootForce;

        trajectoryRenderer.DrawTrajectory(shootPoint.position, velocity);
    }

    public void ShootArrow()
    {
        if (arrowPrefab == null || shootPoint == null || Camera.main == null) return;

        PlayerArrow arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity).GetComponent<PlayerArrow>();
        arrow.ArrowInit(30f, ArrowType.Normal, _player.gameObject);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb == null) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 target = Physics.Raycast(ray, out RaycastHit hit, 100f) ? hit.point : ray.origin + ray.direction * 100f;

        Vector3 direction = (target - shootPoint.position).normalized;
        rb.linearVelocity = direction * shootForce;

        arrow.transform.rotation = Quaternion.LookRotation(direction);
        arrow.transform.Rotate(90f, 0f, 0f, Space.Self);
    }

    public override void OnAttackAnimationEnd()
    {
        ResetAttack();
    }

    public override void EnableComboInput()
    {
        _canShootNext = true;
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 dir) { }

    public override void OnAttackEffectPlay() { }

}