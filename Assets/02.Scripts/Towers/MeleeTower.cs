using System.Collections.Generic;
using UnityEngine;

public class MeleeTower : MonoBehaviour
{
    [Header("데이타")]
    [SerializeField] protected SO_TowerData _data;
    public SO_TowerData Data => _data;

    protected float _currentHealth;
    protected float _attackTimer;

    private readonly List<IDamageAble> _targets = new();

    private void OnEnable()
    {
        _currentHealth = _data.Health;
        _attackTimer = 0f;
        _targets.Clear();
    }

    private void Update()
    {
        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0f)
        {
            _attackTimer = _data.AttackRate;

            foreach (var target in _targets.ToArray()) // ToArray()는 리스트 중간 제거 보호용
            {
                if (target == null) continue;

                target.TakeDamage(new Damage(_data.Damage, gameObject, 10, Vector3.zero));
                _currentHealth -= 20;

                if (_currentHealth <= 0f)
                {
                    ObjectPool.Instance.ReturnToPool(gameObject);
                    return;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out var damageAble))
        {
            if (!_targets.Contains(damageAble))
                _targets.Add(damageAble);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out var damageAble))
        {
            _targets.Remove(damageAble);
        }
    }
}
