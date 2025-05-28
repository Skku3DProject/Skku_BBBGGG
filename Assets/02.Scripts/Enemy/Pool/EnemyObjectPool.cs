using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool
{
    private Stack<GameObject> _pool; // Queue���� Stack�� �� ����
    private GameObject _prefab;
    private Transform _parent;
    private int _createCount; // ������ �� ���� ����

    public EnemyObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        _pool = new Stack<GameObject>(initialSize);
        _createCount = 0;

        // �ʱ� ������Ʈ ����
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = UnityEngine.Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        obj.name = $"{_prefab.name}_{_createCount}"; // ������
        _pool.Push(obj);
        _createCount++;
        return obj;
    }
    public GameObject Get(Vector3 position = default)
    {
        GameObject obj = null;

        if (_pool.Count > 0)
        {
            obj = _pool.Pop();
        }
        else
        {
            obj = CreateNewObject();
            Debug.Log("CreateNewObject");
        }

        if (obj != null)
        {
            obj.TryGetComponent<IEnemyPoolable>(out var poolable);
            poolable?.OnSpawn();
            obj.transform.position = position;
            obj.gameObject.SetActive(true);
        }

        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;
        obj.gameObject.SetActive(false);

        obj.TryGetComponent<IEnemyPoolable>(out var poolable);
        poolable?.OnDespawn();

        _pool.Push(obj);
    }

    public int AvailableCount => _pool.Count;
    public int TotalCreated => _createCount;
}

// ���׸� �����
//public class EnemyObejctPool<T> where T : Component, IEnemyPoolable
//{
//    private Stack<T> _pool; // Queue���� Stack�� �� ����
//    private T _prefab;
//    private Transform _parent;
//    private int _maxSize;
//    private int _createCount; // ������ �� ���� ����


//    public EnemyObejctPool(T prefab, int initialSize, Transform parent = null, int maxSize = 100)
//    {
//        _prefab = prefab;
//        _parent = parent;
//        _maxSize = maxSize;
//        _pool = new Stack<T>(initialSize);
//        _createCount = 0;

//        // �ʱ� ������Ʈ ����
//        for (int i = 0; i < initialSize; i++)
//        {
//            CreateNewObject();
//        }
//    }

//    private T CreateNewObject()
//    {
//        if (_createCount >= _maxSize)
//            return null;

//        T obj = UnityEngine.Object.Instantiate(_prefab, _parent);
//        obj.gameObject.SetActive(false);
//        obj.name = $"{_prefab.name}_{_createCount}"; // ������
//        _pool.Push(obj);
//        _createCount++;
//        return obj;
//    }

//    public T Get()
//    {
//        T obj = null;

//        if (_pool.Count > 0)
//        {
//            obj = _pool.Pop();
//        }
//        else
//        {
//            obj = CreateNewObject();
//            Debug.Log("CreateNewObject");
//        }

//        if (obj != null)
//        {
//            obj.gameObject.SetActive(true);
//            obj.OnSpawn();
//        }

//        return obj;
//    }

//    public void ReturnToPool(T obj)
//    {
//        if (obj == null) return;

//        obj.OnDespawn();
//        obj.ResetState();
//        obj.gameObject.SetActive(false);

//        if (_pool.Count < _maxSize)
//        {
//            _pool.Push(obj);
//        }
//        else
//        {
//            UnityEngine.Object.Destroy(obj.gameObject);
//            _createCount--;
//        }
//    }

//    public int AvailableCount => _pool.Count;
//    public int TotalCreated => _createCount;
//}
