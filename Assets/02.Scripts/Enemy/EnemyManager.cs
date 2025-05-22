using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    // 활성화된 적 리스트
    private readonly List<Enemy> _activeEnemies = new List<Enemy>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy))
            _activeEnemies.Add(enemy);
    }

    public void Unregister(Enemy enemy)
    {
        if (_activeEnemies.Count <= 0) return;
        _activeEnemies.Remove(enemy);
    }

    public bool TryCheckRegister(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy))
        {
            return false;
        }
        return true;
    }

    // 주어진 적을 제외한 활성화된 모든 이웃 리스트 반환
    public List<Enemy> GetNeighbors(Enemy self)
    {
        List<Enemy> neighbors = new List<Enemy>();
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != self)
            {
                neighbors.Add(enemy);
            }
        }
        return neighbors;
    }
}
