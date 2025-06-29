using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    // 활성화된 적 리스트
    private List<Enemy> _activeEnemies;
    public List<Enemy> ActiveEnemies => _activeEnemies;

    private List<IEnemyGroupingStrategy> _groupStrategies;

    public IEnemyGroupingStrategy GroupingStrategy { get; private set; }

    private Dictionary<Enemy, IEnemyGroupingStrategy> _enemyStrategies = new();
    private MoveTypeGroupingStrategy _moveTypeStrategy = new();
    private TargetBasedGroupingStrategy _targetStrategy = new();

    private List<Vector3> _spawnPositionList;
    public List<Vector3> SpawnPositionList => _spawnPositionList;

    private List<Enemy> _summonEnemies;
    public List<Enemy> SummonEnemies => _summonEnemies;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this);

    }
    public void SetEnemiesList(int capacity)
    {
        _activeEnemies = new List<Enemy>(capacity);
        _summonEnemies = new List<Enemy>(capacity);
    }

    public void SpawnerMonsterPositionList(List<Vector3> spawnPositionList)
    {
        _spawnPositionList = spawnPositionList;
    }

    public void OnActivity(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy))
            _activeEnemies.Add(enemy);
    }

    public void UnActivity(Enemy enemy)
    {
        if (_activeEnemies.Count <= 0) return;
        _activeEnemies.Remove(enemy);

        if (_enemyStrategies.TryGetValue(enemy, out var strategy))
        {
            strategy.LeaveGroup(enemy);
            _enemyStrategies.Remove(enemy);
        }

        _summonEnemies.Add(enemy);

		UIManager.instance.CurrentCountRefresh();
    }

    public void SummonEnemiesClear()
    {
        _summonEnemies.Clear();
    }


	public void SetMoveTypeGrouping(Enemy enemy)
    {
        SetGrouping(enemy, _moveTypeStrategy);
    }

    public void SetTargetGrouping(Enemy enemy)
    {
        SetGrouping(enemy, _targetStrategy);
    }

    public void ClearGrouping(Enemy enemy)
    {
        if (_enemyStrategies.TryGetValue(enemy, out var strategy))
        {
            strategy.LeaveGroup(enemy);
            _enemyStrategies.Remove(enemy);
        }
    }

    private void SetGrouping(Enemy enemy, IEnemyGroupingStrategy newStrategy)
    {
        if (_enemyStrategies.TryGetValue(enemy, out var oldStrategy))
        {
            oldStrategy.LeaveGroup(enemy);
        }

        newStrategy.JoinGroup(enemy);
        _enemyStrategies[enemy] = newStrategy;
    }

    public bool TryCheckRegister(Enemy enemy)
    {
        if (_enemyStrategies.TryGetValue(enemy, out var strategy))
        {
            return strategy is MoveTypeGroupingStrategy;
        }
        return false;
    }

    public List<Enemy> GetNeighbors(Enemy enemy, float neighborRadius)
    {
        if (!_enemyStrategies.TryGetValue(enemy, out var strategy))
            return new List<Enemy>();

        List<Enemy> rawGroup = strategy.GetGroupMembers(enemy);
        List<Enemy> filtered = new();

        foreach (var other in rawGroup)
        {
            if (other == enemy) continue;

            float dist = Vector3.Distance(enemy.transform.position, other.transform.position);
            if (dist <= neighborRadius)
                filtered.Add(other);
        }

        return filtered;
    }
}
