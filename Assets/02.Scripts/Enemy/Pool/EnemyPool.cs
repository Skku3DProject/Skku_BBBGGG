using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class EnemyPoolContainer
{
    public string Key;
    public GameObject Parent;
    public GameObject PoolingPrefab;
    public int Size;
}

public class EnemyPool : MonoBehaviour
{
    public List<EnemyPoolContainer> PoolPrefabs;

    public static EnemyPool Instace;

    private void Awake()
    {
        if(Instace == null)
        {
            Instace = this;
            Pool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Pool()
    {

        foreach (var poolPrefab in PoolPrefabs)
        {
            List<GameObject> Prefabs = new List<GameObject>(poolPrefab.Size);
            for (int i = 0; i < poolPrefab.Size; i++)
            {
                Prefabs.Add(Instantiate(poolPrefab.PoolingPrefab, poolPrefab.PoolingPrefab.transform));
            }
        }
    }
    /*
    public T GetList<T> (string className)
    {
        if(!SpawnObjects.ContainsKey(className))
        {
            Debug.Log("클래스 이름이 다릅니다.");
            return default;
        }

        return SpawnObjects[className];
    }*/
}
