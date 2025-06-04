using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public float damageAmount = 9999f; // ��� ������ �Ǵ� ���� ������

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageAble damageTarget))
        {
            damageTarget.TakeDamage(new Damage(damageAmount,gameObject));
        }

    }
}
