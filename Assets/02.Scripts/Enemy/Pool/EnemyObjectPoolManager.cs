using System.Collections.Generic;
using UnityEngine;
public class EnemyObjectPoolManager : MonoBehaviour
{
    public static EnemyObjectPoolManager Instance { get; private set; }
    // 타입별 전용 풀들 (컴파일 타임에 결정)
    private Dictionary<string, EnemyObjectPool> _pools = new Dictionary<string, EnemyObjectPool>();
    [Header("Pool Settings")]
    [SerializeField] private List<EnemyPoolContainer> _poolSettings;

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
        foreach (EnemyPoolContainer poolSetting in _poolSettings)
        {
            if (poolSetting.prefab != null && !string.IsNullOrEmpty(poolSetting.key))
            {
                // Key별로 풀 생성 및 등록
                var pool = new EnemyObjectPool(
                    poolSetting.prefab,
                    poolSetting.initialSize,
                    poolSetting.parent,
                    poolSetting.maxSize
                );

                _pools[poolSetting.key] = pool;
                Debug.Log($"Pool created for key: {poolSetting.key}");
            }
            else
            {
                Debug.LogWarning($"Invalid pool setting: {poolSetting.key}");
            }
        }
    }

    // Key를 통한 오브젝트 가져오기
    public GameObject GetObject(string key)
    {
        if (_pools.TryGetValue(key, out EnemyObjectPool pool))
        {
            return pool.Get();
        }

        Debug.LogWarning($"Pool with key '{key}' not found!");
        return null;
    }

    // Key를 통한 오브젝트 반환하기
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

    // 특정 풀의 상태 정보 가져오기
    public (int available, int total) GetPoolInfo(string key)
    {
        if (_pools.TryGetValue(key, out EnemyObjectPool pool))
        {
            return (pool.AvailableCount, pool.TotalCreated);
        }

        return (0, 0);
    }

    // 등록된 모든 풀 키 목록 가져오기
    public List<string> GetAllPoolKeys()
    {
        return new List<string>(_pools.Keys);
    }

    // 특정 풀이 존재하는지 확인
    public bool HasPool(string key)
    {
        return _pools.ContainsKey(key);
    }

    // 런타임에 새로운 풀 추가 (선택사항)
    public void AddPool(string key, GameObject prefab, int initialSize = 10, Transform parent = null, int maxSize = 100)
    {
        if (_pools.ContainsKey(key))
        {
            Debug.LogWarning($"Pool with key '{key}' already exists!");
            return;
        }

        var pool = new EnemyObjectPool(prefab, initialSize, parent, maxSize);
        _pools[key] = pool;
        Debug.Log($"Runtime pool created for key: {key}");
    }
}
