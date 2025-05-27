using System.Collections.Generic;
using UnityEngine;

public class EnemyFlocking
{
    public Vector3 CalculateSeparation(Vector3 pos, Enemy enemy)
    {
        if (!EnemyManager.Instance.TryCheckMoveRegister(enemy))
        {
            return Vector3.zero;
        }

        List<Enemy> neighbors = EnemyManager.Instance.GetNeighbors(enemy,enemy.EnemyData.FlockNeighborRadius);

        Vector3 force = Vector3.zero;
        int count = 0;

        foreach (Enemy other in neighbors)
        {
            if (other == enemy) continue;

            Vector3 diff = pos - other.transform.position;
            float dist = diff.magnitude;

            if (dist < enemy.EnemyData.SeparationDistance && dist > 0.001f)
            {
                force += diff.normalized / dist;
                count++;
            }
        }

        if (count > 0)
        {
            force /= count;
        }

        return force.normalized;
    }

    public Vector3 CalculateCohesion(Vector3 pos, Enemy enemy)
    {
        if (!EnemyManager.Instance.TryCheckMoveRegister(enemy))
        {
            return Vector3.zero;
        }

        List<Enemy> neighbors = EnemyManager.Instance.GetNeighbors(enemy, enemy.EnemyData.FlockNeighborRadius);
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (Enemy other in neighbors)
        {
            if (other == enemy) continue;
            center += other.transform.position;
            count++;
        }

        if (count > 0)
        {
            center /= count;
            return (center - pos).normalized;
        }

        return Vector3.zero;
    }
    public Vector3 CalculateAlignment(Enemy enemy)
    {
        if (!EnemyManager.Instance.TryCheckMoveRegister(enemy))
        {
            return Vector3.zero;
        }

        List<Enemy> neighbors = EnemyManager.Instance.GetNeighbors(enemy, enemy.EnemyData.FlockNeighborRadius);
        Vector3 averageDir = Vector3.zero;
        int count = 0;

        foreach (Enemy other in neighbors)
        {
            if (other == enemy) continue;
            averageDir += other.CurrentMoveDirection;
            count++;
        }

        if (count > 0)
        {
            averageDir /= count;
            return averageDir.normalized;
        }

        return Vector3.zero;
    }
}
