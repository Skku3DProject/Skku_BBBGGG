using UnityEngine;

public class SpikeMeleeTower : MeleeTowerBase
{
    [SerializeField] private float knockbackForce = 10f;

    protected override void AttackTargets()
    {
        // ToArray()로 복사본을 만들어 안전하게 순회
        var targetArray = _targets.ToArray();

        foreach (var target in targetArray)
        {
            if (target == null) continue;

            target.TakeDamage(new Damage(_data.Damage, gameObject, knockbackForce));

        }
        _currentHealth -= 20;
        if (_currentHealth <= 0) ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
