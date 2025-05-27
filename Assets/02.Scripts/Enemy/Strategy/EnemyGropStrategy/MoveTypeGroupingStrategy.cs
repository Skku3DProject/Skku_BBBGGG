using System.Collections.Generic;
using UnityEngine;

public class MoveTypeGroupingStrategy : IEnemyGroupingStrategy
{
    private Dictionary<EEnemyMoveType, List<Enemy>> _groupDict = new();

    public void JoinGroup(Enemy enemy)
    {
        var type = enemy.EnemyData.EnemyMoveType;
        if (!_groupDict.ContainsKey(type))
        {
            _groupDict[type] = new List<Enemy>();
        }

        if (!_groupDict[type].Contains(enemy))
            _groupDict[type].Add(enemy);
    }

    public void LeaveGroup(Enemy enemy)
    {
        var type = enemy.EnemyData.EnemyMoveType;
        if (_groupDict.ContainsKey(type))
        {
            _groupDict[type].Remove(enemy);
        }
    }

    public List<Enemy> GetGroupMembers(Enemy enemy)
    {
        var type = enemy.EnemyData.EnemyMoveType;
        if (_groupDict.TryGetValue(type, out var list))
        {
            return list.FindAll(e => e != enemy);
        }

        return new List<Enemy>();
    }
}
