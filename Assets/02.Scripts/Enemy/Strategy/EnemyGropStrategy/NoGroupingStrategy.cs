using System.Collections.Generic;
using UnityEngine;

public class NoGroupingStrategy : IEnemyGroupingStrategy
{
    public void JoinGroup(Enemy enemy) { } // ����
    public void LeaveGroup(Enemy enemy) { } // ����
    public List<Enemy> GetGroupMembers(Enemy enemy) => new List<Enemy>(); // �� ����Ʈ ��ȯ
}
