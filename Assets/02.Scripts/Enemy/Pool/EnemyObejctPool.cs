using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyObejctPool<T> where T : MonoBehaviour, IPool
{
    private Queue<T> _pool = new Queue<T>();
    private T _prefab;
    private Transform _parent;

    public EnemyObejctPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        T obj = _pool.Count > 0 ? _pool.Dequeue() : GameObject.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(true);
        obj.OnSpawn();

        return obj;
    }

    public void ReturnToPool(T obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}
