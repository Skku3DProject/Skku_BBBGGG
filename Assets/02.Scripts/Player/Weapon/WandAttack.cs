using UnityEngine;

public class WandAttack : WeaponAttackBase
{
    [Header("Projectile Settings")]
    public GameObject magicProjectilePrefab;
    public Transform shootPoint;
    public float projectileForce = 25f;
    public float attackCooldown = 0.5f;


    private PlayerAttack _playerAttack;
    private bool _canAttack = true;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerAttack = GetComponent<PlayerAttack>();

    }

    public override void Attack()
    {
        if (!_canAttack || magicProjectilePrefab == null || shootPoint == null)
            return;

        _canAttack = false;
        IsAttacking = true;

        _playerAttack.IsUsingJumpAnim = false;

        _animator.SetTrigger("Attack"); // �ִϸ��̼ǿ��� �߻� Ÿ�̹� ���߷��� Delay ���
        Invoke(nameof(ResetAttackCooldown), attackCooldown);
    }

    private void FireProjectile()
    {
        PlayerSoundController.Instance.PlaySound(PlayerSoundType.WandAttack);

        GameObject projectile = Instantiate(magicProjectilePrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // 크로스헤어 기준 Raycast
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            targetPoint = hit.point; // 명중한 위치
        else
            targetPoint = ray.origin + ray.direction * 100f; // 그냥 정면

        Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;

        // 발사
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = shootDirection * projectileForce;
        }

        float power = PlayerEquipmentController.Instance.GetCurrentWeaponAttackPower();
        MagicProjectile proj = projectile.GetComponent<MagicProjectile>();
        proj?.Init(power, gameObject);

        projectile.transform.rotation = Quaternion.LookRotation(shootDirection); // 시각 방향 정렬
    }

    private void ResetAttackCooldown()
    {

        IsAttacking = false;
        _canAttack = true;
        _playerAttack.IsUsingJumpAnim = true;

    }

    public override void EnableComboInput()
    {

    }

    public override void OnAttackAnimationEnd()
    {
    }

    public override void OnAttackEffectPlay()
    {
    }

    public override void TryDamageEnemy(GameObject enemy, Vector3 dir)
    {
    }
}
