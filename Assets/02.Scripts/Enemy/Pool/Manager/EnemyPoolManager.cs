using System.Collections.Generic;
using UnityEngine;
public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }
    // Ÿ�Ժ� ���� Ǯ�� (������ Ÿ�ӿ� ����)
    private Dictionary<string, EnemyObjectPool> _pools = new Dictionary<string, EnemyObjectPool>();
   [Header("Pool Settings")]
    public So_EnemyPool So_PoolData;

    private int _maxEnemyCount = 0;
    public int MaxEnemyCount => _maxEnemyCount;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        foreach (EnemyPoolContainer poolSetting in So_PoolData.PoolSettings)
        {
            if (poolSetting.prefab != null && !string.IsNullOrEmpty(poolSetting.key))
            {
                // Key���� Ǯ ���� �� ���
                var pool = new EnemyObjectPool(
                    poolSetting.prefab,
                    poolSetting.initialSize,
                    this.gameObject.transform
                );

                _pools[poolSetting.key] = pool;
                _maxEnemyCount += poolSetting.initialSize;
            }
            else
            {
                Debug.LogWarning($"Invalid pool setting: {poolSetting.key}");
            }
        }
    }

    // Key�� ���� ������Ʈ ��������
    public GameObject GetObject(string key)
    {
        if (_pools.TryGetValue(key, out EnemyObjectPool pool))
        {
            return pool.Get();
        }

        Debug.LogWarning($"Pool with key '{key}' not found!");
        return null;
    }

    // Key�� ���� ������Ʈ ��ȯ�ϱ�
    public void ReturnObject(string key, GameObject gameObject)
    {
        if (_pools.TryGetValue(key, out EnemyObjectPool pool))
        {
            pool.ReturnToPool(gameObject);
        }
        else
        {
            Debug.LogWarning($"Pool with key '{key}' not found for return!");
        }
    }

    // Ư�� Ǯ�� ���� ���� ��������
    public (int available, int total) GetPoolInfo(string key)
    {
        if (_pools.TryGetValue(key, out EnemyObjectPool pool))
        {
            return (pool.AvailableCount, pool.TotalCreated);
        }

        return (0, 0);
    }

    // ��ϵ� ��� Ǯ Ű ��� ��������
    public List<string> GetAllPoolKeys()
    {
        return new List<string>(_pools.Keys);
    }

    // Ư�� Ǯ�� �����ϴ��� Ȯ��
    public bool HasPool(string key)
    {
        return _pools.ContainsKey(key);
    }

    // ��Ÿ�ӿ� ���ο� Ǯ �߰� (���û���)
    public void AddPool(string key, GameObject prefab, int initialSize = 10, Transform parent = null)
    {
        if (_pools.ContainsKey(key))
        {
            Debug.LogWarning($"Pool with key '{key}' already exists!");
            return;
        }

        var pool = new EnemyObjectPool(prefab, initialSize, parent);
        _pools[key] = pool;
        Debug.Log($"Runtime pool created for key: {key}");
    }
}
