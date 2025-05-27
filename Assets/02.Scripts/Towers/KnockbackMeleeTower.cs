using UnityEngine;

public class KnockbackMeleeTower : MeleeTowerBase
{
    [SerializeField] private float knockbackForce = 10f;

    protected override void AttackTargets()
    {
        foreach (var target in _targets.ToArray())
        {
            if (target == null) continue;
            // MonoBehaviour�� ĳ�����ؼ� transform ���� ����
            if (target is MonoBehaviour targetMB)
            {
                // �˹� ���� ���
                Vector3 knockbackDir = (targetMB.transform.position - transform.position).normalized;

                // TakeDamage ȣ�� �� �˹� ���� ����
                target.TakeDamage(new Damage(_data.Damage, gameObject, knockbackForce, knockbackDir));
            }
        }
        _currentHealth -= 20;
        if (_currentHealth <= 0) ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
