using System.Collections.Generic;
using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{
    public Transform NearEnemy { private set; get; }
    private readonly List<Transform> _targets = new();



    public bool CanAttakc = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _targets.Add(other.transform);
            UpdateNearEnemy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _targets.Remove(other.transform);
            UpdateNearEnemy();
        }
    }

    private void Update()
    {
        UpdateNearEnemy();
        if (NearEnemy != null) CanAttakc = true;
        else CanAttakc = false;

    }

    private void UpdateNearEnemy()
    {
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (var target in _targets)
        {
            if (target == null) continue;

            float dist = Vector3.Distance(transform.position, target.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = target;
            }
        }

        NearEnemy = closest;
    }
}
