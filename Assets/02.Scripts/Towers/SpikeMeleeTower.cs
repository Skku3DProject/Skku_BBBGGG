using UnityEngine;

public class SpikeMeleeTower : MeleeTowerBase
{
    [SerializeField] private float knockbackForce = 10f;

    protected override void AttackTargets()
    {
        foreach (var target in _targets.ToArray())
        {
            if (target == null) continue;

            target.TakeDamage(new Damage(_data.Damage, gameObject, knockbackForce, Vector3.zero));


        }
        _currentHealth -= 20;
        if (_currentHealth <= 0) ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
