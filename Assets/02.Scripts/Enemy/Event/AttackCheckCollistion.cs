using UnityEngine;

public class AttackCheckCollistion : MonoBehaviour
{
    private Enemy _enemy;
    public CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DamageAble>(out var dmg) && other.CompareTag("Player"))
        {
            dmg.TakeDamage(new Damage(_enemy.EnemyData.Power, _enemy.gameObject, _enemy.EnemyData.KnockbackPower));
            _capsuleCollider.enabled = false;
        }
    }
}
