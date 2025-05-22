using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    // Ȱ��ȭ�� �� ����Ʈ
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

    // �־��� ���� ������ Ȱ��ȭ�� ��� �̿� ����Ʈ ��ȯ
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
