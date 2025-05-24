using System.Collections.Generic;
using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{
    public GameObject NearEnemy { private set; get; }
    public readonly List<GameObject> _targets = new();
    private float _updateTimer;
    private TowerBase _owner;

    public bool CanAttakc = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _targets.Add(other.gameObject);
            UpdateNearEnemy();
        }
    }

    private void Start()
    {
        _owner = GetComponentInParent<TowerBase>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _targets.Remove(other.gameObject);
            UpdateNearEnemy();
        }
    }

    private void Update()
    {
        _updateTimer -= Time.deltaTime;
        if (_updateTimer < 0f)
        {
            _updateTimer = 0.5f;
            UpdateNearEnemy();
        }

        CanAttakc = (NearEnemy != null && NearEnemy.activeSelf);
    }

    private void UpdateNearEnemy()
    {
        float minDist = float.MaxValue;
        GameObject closest = null;

        // 리스트 클린업
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            if (_targets[i] == null || !_targets[i].activeSelf)
            {
                _targets.RemoveAt(i);
            }
        }

        foreach (var target in _targets)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < minDist || dist >= _owner.Data.MinRange)
            {
                minDist = dist;
                closest = target;
            }
        }

        NearEnemy = closest;
    }
}
