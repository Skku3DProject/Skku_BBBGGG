using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    // ���̺� ���� ������ ����, ���� ��ȯ

    public List<GameObject> PreSetList; 


    private List<GameObject> _spawnList;
    public List<GameObject> SpwanList => _spawnList;

    public EnemySpawnerSo SpawnerSo;

    public int SpawnCount = 10;

    private void Awake()
    {
        _spawnList = new List<GameObject>(SpawnCount * PreSetList.Count);

        for(int i =0; i< PreSetList.Count; i++)
        {
            for (int j = 0; j < SpawnCount; j++) 
            {
               // Instantiate()
            }
        }
    }

}
