using System.Collections.Generic;
using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{
    public GameObject NearEnemy { private set; get; }
    public readonly List<GameObject> _targets = new();



    public bool CanAttakc = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _targets.Add(other.gameObject);
            UpdateNearEnemy();
        }
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
        UpdateNearEnemy();


        if (_targets.Count > 0)
        {
            if (NearEnemy.activeSelf) { CanAttakc = true; }
            else
            {
                CanAttakc = false;
                NearEnemy = null;
            }

        }
    }

    private void UpdateNearEnemy()
    {
        float minDist = float.MaxValue;
        GameObject closest = null;

        foreach (var target in _targets)
        {
            Debug.Log(target.gameObject.name);
            if (target == null) continue;

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = target;
            }
        }

        NearEnemy = closest;
    }
}
