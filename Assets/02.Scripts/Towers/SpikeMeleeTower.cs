using UnityEngine;

public class SpikeMeleeTower : MeleeTowerBase
{
    [SerializeField] private float knockbackForce = 10f;

    protected override void AttackTargets()
    {
        // ToArray()�� ���纻�� ����� �����ϰ� ��ȸ
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
