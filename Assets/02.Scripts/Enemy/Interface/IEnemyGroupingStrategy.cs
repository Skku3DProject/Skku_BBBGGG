using System.Collections.Generic;
using UnityEngine;

public interface IEnemyGroupingStrategy 
{
    List<Enemy> GetGroupMembers(Enemy enemy);
    void JoinGroup(Enemy enemy);
    void LeaveGroup(Enemy enemy);
   
}
