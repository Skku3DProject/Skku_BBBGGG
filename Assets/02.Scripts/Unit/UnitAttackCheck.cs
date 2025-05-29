using UnityEngine;

public class UnitAttackCheck : MonoBehaviour
{
    private Unit _unit;

    private void Awake()
    {
        _unit = GetComponentInParent<Unit>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy") )
        {
            Vector3 hitPosition = other.ClosestPoint(transform.position); // 충돌 지점 추정
            Vector3 directionToEnemy = (other.transform.position - transform.position).normalized;

            IDamageAble damageable = other.gameObject.GetComponent<IDamageAble>();
            if (damageable != null)
            {
                Damage damage = new Damage(10, _unit.gameObject, 100f, directionToEnemy);
                damageable.TakeDamage(damage);

            }
        }
    }
}
