using System.Collections.Generic;
using UnityEngine;

// 필요할것같을때 사용
public abstract class EnemyBastState : IFSM
{
    protected Enemy _enemy;


    GameObject[] others = GameObject.FindGameObjectsWithTag("enemyTag");

    protected Vector3 CalculateSeparation()
    {
        Vector3 force = Vector3.zero;
        int count = 0;
        Vector3 myPos = _enemy.transform.position;

        foreach (var go in others)
        {
            if (go == _enemy.gameObject) continue;
            Vector3 dir = myPos - go.transform.position;
            float dist = dir.magnitude;
            if (dist < _enemy.EnemyData.SeparationDistance && dist > 0.001f)
            {
                force += dir.normalized / (dist + 0.001f);
                count++;
            }
        }
        if (count > 0) force /= count;
        return force.normalized;
    }

    public abstract void Start();
    public abstract EEnemyState Update();
    public abstract void End();
}
