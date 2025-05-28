using System;
using UnityEngine;

[System.Serializable]
public class EnemyPoolContainer 
{
    public string key;
    public GameObject prefab;
    public int initialSize = 10;
    public int maxSize = 50;
    public Transform parent;
}
