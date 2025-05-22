using UnityEngine;

public class AttackCheckCollistion : MonoBehaviour
{
    private Damage _damage;
    public CapsuleCollider _capsuleCollider;

    public void Initialized(Damage damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out var dmg) && other.CompareTag("Player"))
        {
            dmg.TakeDamage(_damage);
            gameObject.SetActive(false);
        }
    }
}
