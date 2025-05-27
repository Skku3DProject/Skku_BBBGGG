using UnityEngine;

public class KnockbackMeleeTower : MeleeTowerBase
{
    [SerializeField] private float knockbackForce = 10f;

    protected override void AttackTargets()
    {
        foreach (var target in _targets.ToArray())
        {
            if (target == null) continue;
            // MonoBehaviour로 캐스팅해서 transform 정보 접근
            if (target is MonoBehaviour targetMB)
            {
                // 넉백 방향 계산
                Vector3 knockbackDir = (targetMB.transform.position - transform.position).normalized;

                // TakeDamage 호출 시 넉백 정보 전달
                target.TakeDamage(new Damage(_data.Damage, gameObject, knockbackForce, knockbackDir));
            }
        }
        _currentHealth -= 20;
        if (_currentHealth <= 0) ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
