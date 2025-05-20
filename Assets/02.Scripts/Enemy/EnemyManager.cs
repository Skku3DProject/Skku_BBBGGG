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

    /// <summary>�� Ǯ�� �߰�</summary>
    public void Register(Enemy e)
    {
        if (!_activeEnemies.Contains(e))
            _activeEnemies.Add(e);
    }

    /// <summary>�� Ǯ���� ����</summary>
    public void Unregister(Enemy e)
    {
        if (_activeEnemies.Count <= 0) return;
        _activeEnemies.Remove(e);
    }

    /// <summary>
    /// �־��� ���� ������ Ȱ��ȭ�� ��� �̿� ����Ʈ ��ȯ
    /// </summary>
    public List<Enemy> GetNeighbors(Enemy self)
    {
        // �ʿ信 ���� �Ÿ� ���͸� �� �߰� ����
        return _activeEnemies;
    }
}
