using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPoolManger : MonoBehaviour
{
    public static EnemyObjectPoolManger Instance { get; private set; }
    // 타입별 전용 풀들 (컴파일 타임에 결정)
    private Dictionary<string, EnemyObjectPool> _pools = new Dictionary<string, EnemyObjectPool>();
    [Header("Pool Settings")]
    public So_EnemyPool So_PoolData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        
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
                // Key별로 풀 생성 및 등록
                var pool = new EnemyObjectPool(
                    poolSetting.prefab,
                    poolSetting.initialSize
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
    public GameObject GetObject(string key , Vector3 position)
    {
        if (_pools.TryGetValue(key, out EnemyObjectPool pool))
        {
            return pool.Get(position);
        }

        Debug.LogWarning($"Pool with key '{key}' not found!");
        return null;
    }

    // Key를 통한 오브젝트 반환하기
    public void ReturnObject(string key, GameObject poolObject)
    {
        if (poolObject == null) return;

        if (_pools.TryGetValue(key, out EnemyObjectPool pool))
        {
            if (!poolObject.activeInHierarchy)
            {
                Debug.LogWarning($"Object {poolObject.name} is already inactive!");
                return;
            }

            //poolObject.transform.SetParent(this.gameObject.transform);
            pool.ReturnToPool(poolObject);
        }
        else
        {
            Destroy(poolObject);
            Debug.LogWarning($"Pool with key '{key}' not found for return!");
        }
    }

    // 특정 풀이 존재하는지 확인
    public bool HasPool(string key)
    {
        return _pools.ContainsKey(key);
    }

    // 런타임에 새로운 풀 추가 (선택사항)
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
