using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [Header("Pool Entries")]
    public List<PoolEntry> poolEntries;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();
    private Dictionary<GameObject, int> objSizeDictionary = new();
    private Dictionary<string, Transform> parentDictionary = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // 씬로딩용 코루틴
    public IEnumerator InitPoolAllAsync(Action<float> onProgress)
    {
        // 총 생성할 오브젝트 개수 계산
        int total = 0;
        foreach (var entry in poolEntries)
            if (entry.prefab != null)
                total += Mathf.Max(1, entry.size);

        int created = 0;

        // 각 엔트리별로 큐 초기화
        foreach (var entry in poolEntries)
        {
            var prefab = entry.prefab;
            if (prefab == null) continue;

            int count = Mathf.Max(1, entry.size);
            poolDictionary[prefab] = new Queue<GameObject>();

            // 부모 트랜스폼 준비
            string key = prefab.name;
            if (!parentDictionary.ContainsKey(key))
            {
                var parent = new GameObject(key + "_Pool");
                parent.transform.SetParent(transform);
                parentDictionary[key] = parent.transform;
            }

            // 오브젝트 생성
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(prefab);
                go.transform.SetParent(parentDictionary[key]);
                go.AddComponent<PooledObject>().OrginPrefab = prefab;
                go.SetActive(false);
                poolDictionary[prefab].Enqueue(go);

                created++;
                onProgress?.Invoke((float)created / total);
                yield return null; // 프레임 분산
            }
        }
        // 완료 보정
        onProgress?.Invoke(1f);
    }
    //private void Start()
    //{
    //    foreach (var entry in poolEntries)
    //    {
    //        if (entry.prefab == null) continue;

    //        objSizeDictionary[entry.prefab] = entry.size;
    //        InitPool(entry.prefab);
    //    }
    //}

    private void InitPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        if (!objSizeDictionary.ContainsKey(prefab))
            objSizeDictionary[prefab] = 50;

        for (int i = 0; i < objSizeDictionary[prefab]; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        string objName = prefab.name;

        if (!parentDictionary.ContainsKey(objName))
        {
            GameObject parentObj = new GameObject(objName);
            parentObj.transform.SetParent(transform);
            parentDictionary[objName] = parentObj.transform;
        }

        GameObject newObj = Instantiate(prefab);
        newObj.transform.SetParent(parentDictionary[objName]);
        newObj.AddComponent<PooledObject>().OrginPrefab = prefab;
        newObj.SetActive(false);
        poolDictionary[prefab].Enqueue(newObj);
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            InitPool(prefab);
        }

        if (poolDictionary[prefab].Count == 0)
        {
            CreateNewObject(prefab);
        }

        var obj = poolDictionary[prefab].Dequeue();
        obj.SetActive(true);
        return obj;
    }
    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            InitPool(prefab);
        }

        if (poolDictionary[prefab].Count == 0)
        {
            CreateNewObject(prefab);
        }

        var obj = poolDictionary[prefab].Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }
    public void ReturnToPool(GameObject objectToReturn)
    {
        var origin = objectToReturn.GetComponent<PooledObject>().OrginPrefab;
        objectToReturn.SetActive(false);
        poolDictionary[origin].Enqueue(objectToReturn);
    }
}
