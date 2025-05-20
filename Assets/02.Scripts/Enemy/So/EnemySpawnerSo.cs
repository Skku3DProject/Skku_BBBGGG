using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerSo", menuName = "Scriptable Objects/EnemySpawnerSo")]
public class EnemySpawnerSo : ScriptableObject
{
    public List<EEnemyType> EnemyTypes;
    public List<EEnemyAttackType> EnemyAttackTypes;

    public List<int> SpawnCounts;
}
