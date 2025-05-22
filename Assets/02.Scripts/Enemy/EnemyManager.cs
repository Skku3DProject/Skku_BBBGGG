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

    public void Register(Enemy e)
    {
        if (!_activeEnemies.Contains(e))
            _activeEnemies.Add(e);
    }

    public void Unregister(Enemy e)
    {
        if (_activeEnemies.Count <= 0) return;
        _activeEnemies.Remove(e);
    }

    // 주어진 적을 제외한 활성화된 모든 이웃 리스트 반환
    public List<Enemy> GetNeighbors(Enemy self)
    {
        // 필요에 따라 거리 필터링 등 추가 가능
        return _activeEnemies;
    }
}
