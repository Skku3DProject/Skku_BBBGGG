using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private readonly Dictionary<EEnemyAttackType,List<Enemy>> _aggregationEnemyDictionary = new Dictionary<EEnemyAttackType, List<Enemy>>();
    // 활성화된 적 리스트
    private  List<Enemy> _activeEnemies;
    public  List<Enemy> ActiveEnemies => _activeEnemies;

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

    public void SetEnemiesList(int capacity)
    {
        _activeEnemies = new List<Enemy>(capacity);
    }

    public void Enable(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy))
            _activeEnemies.Add(enemy);
    }

    public void UnEnable(Enemy enemy)
    {
        if (_activeEnemies.Count <= 0) return;
        _activeEnemies.Remove(enemy);
        UIManager.instance.CurrentCountRefresh();
    }

    public void Register(Enemy enemy, EEnemyAttackType type)
    {
        /*
        if (_aggregationEnemyDictionary.Count <= 0) return;
        _aggregationEnemyDictionary[type].Remove(enemy);
       */
    }

    public void Unregister(Enemy enemy, EEnemyAttackType type)
    {
        /*
        if (!_aggregationEnemyDictionary[type].Contains(enemy))
            _aggregationEnemyDictionary[type].Add(enemy);
      */
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
