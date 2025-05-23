using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private  List<Enemy> _curruntEnemyList = new List<Enemy>(900);
    public  List<Enemy> CurruntEnemyList => _curruntEnemyList;


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

    public void Enable(Enemy enemy)
    {
        if (!_curruntEnemyList.Contains(enemy))
            _curruntEnemyList.Add(enemy);
    }

    public void UnEnable(Enemy enemy)
    {
        if (_curruntEnemyList.Count <= 0) return;
        _curruntEnemyList.Remove(enemy);
        UIManager.instance.CurrentCountRefresh();
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
