using System.Collections.Generic;
using UnityEngine;

public class NoGroupingStrategy : IEnemyGroupingStrategy
{
    public void JoinGroup(Enemy enemy) { } // 무시
    public void LeaveGroup(Enemy enemy) { } // 무시
    public List<Enemy> GetGroupMembers(Enemy enemy) => new List<Enemy>(); // 빈 리스트 반환
}
