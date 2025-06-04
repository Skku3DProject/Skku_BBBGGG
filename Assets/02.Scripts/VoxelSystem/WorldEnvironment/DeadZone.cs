using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public float damageAmount = 9999f; // 즉사 데미지 또는 낙사 데미지

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageAble damageTarget))
        {
            damageTarget.TakeDamage(new Damage(damageAmount,gameObject));
        }

    }
}
