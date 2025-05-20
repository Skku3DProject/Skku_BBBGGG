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

    private void Start()
    {
        foreach (var entry in poolEntries)
        {
            if (entry.prefab == null) continue;

            objSizeDictionary[entry.prefab] = entry.size;
            InitPool(entry.prefab);
        }
    }

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
