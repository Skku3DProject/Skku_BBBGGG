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

    // �־��� ���� ������ Ȱ��ȭ�� ��� �̿� ����Ʈ ��ȯ
    public List<Enemy> GetNeighbors(Enemy self)
    {
        // �ʿ信 ���� �Ÿ� ���͸� �� �߰� ����
        return _activeEnemies;
    }
}
