using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "So_EnemyPool", menuName = "Scriptable Objects/So_EnemyPool")]
public class So_EnemyPool : ScriptableObject
{
    public List<EnemyPoolContainer> PoolSettings;
}
