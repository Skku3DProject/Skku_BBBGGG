using System.Collections.Generic;
using UnityEngine;

public class TargetBasedGroupingStrategy : IEnemyGroupingStrategy
{
    private Dictionary<GameObject, List<Enemy>> _groupDict = new();

    public void JoinGroup(Enemy enemy)
    {
        var target = enemy.Target;
        if (!_groupDict.ContainsKey(target))
        {
            _groupDict[target] = new List<Enemy>();
        }

        if (!_groupDict[target].Contains(enemy))
            _groupDict[target].Add(enemy);
    }

    public void LeaveGroup(Enemy enemy)
    {
        var target = enemy.Target;
        if (_groupDict.ContainsKey(target))
        {
            _groupDict[target].Remove(enemy);
        }
    }

    public List<Enemy> GetGroupMembers(Enemy enemy)
    {
        var target = enemy.Target;
        if (_groupDict.TryGetValue(target, out var list))
        {
            return list.FindAll(e => e != enemy);
        }

        return new List<Enemy>();
    }
}
