using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerSo", menuName = "Scriptable Objects/EnemySpawnerSo")]
public class So_EnemySpawner : ScriptableObject
{
    public List<EnemySpawnContainer> SpawnEneies;
}
