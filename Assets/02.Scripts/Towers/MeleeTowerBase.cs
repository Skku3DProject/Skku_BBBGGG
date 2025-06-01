using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeTowerBase : MonoBehaviour
{
    [SerializeField] protected SO_TowerData _data;
    public SO_TowerData Data => _data;

    protected float _currentHealth;
    protected float _attackTimer;

    protected readonly List<IDamageAble> _targets = new();

    protected virtual void OnEnable()
    {
        _currentHealth = _data.Health;
        _attackTimer = 0f;
        _targets.Clear();
    }

    protected virtual void Update()
    {
        _targets.RemoveAll(t => t == null); // Á×Àº Àû Á¦°Å

        if (_targets.Count == 0) return;

        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0f)
        {
            _attackTimer = _data.AttackRate;
            AttackTargets();
        }
    }

    protected abstract void AttackTargets();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out var damageAble))
        {
            if (!_targets.Contains(damageAble))
                _targets.Add(damageAble);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IDamageAble>(out var damageAble))
        {
            _targets.Remove(damageAble);
        }
    }
}
