using UnityEngine;

public class FlyEnemyStrategy : IEnemyTypeStrategy
{
    private EnemyFlocking _enemyFlocking;
    private Vector3 _prevDirection = Vector3.zero;
    public FlyEnemyStrategy()
    {
        _enemyFlocking = new EnemyFlocking();
    }

    public void Move(Enemy enemy, float speed)
    {
        Vector3 pos = enemy.transform.position;

        Vector3 goalDir = (enemy.Target.transform.position - pos).normalized;
        Vector3 sepDir = _enemyFlocking.CalculateSeparation(pos, enemy);
        Vector3 cohDir = _enemyFlocking.CalculateCohesion(pos, enemy);
        Vector3 alignDir = _enemyFlocking.CalculateAlignment(enemy);

        float goalWeight = enemy.EnemyData.GoalWeight;
        float sepWeight = enemy.EnemyData.SeparationWeight;
        float cohWeight = enemy.EnemyData.CohesionWeight;
        float alignWeight = enemy.EnemyData.AlignmentWeight;

        Vector3 desiredDir = (goalDir * goalWeight + sepDir * sepWeight + cohDir * cohWeight + alignDir * alignWeight).normalized;

        Vector3 moveDir = Vector3.Lerp(_prevDirection, desiredDir, 0.15f).normalized;
        _prevDirection = moveDir;

        // 5) 최종 이동 및 적용
        enemy.CharacterController.Move(moveDir * speed * Time.deltaTime);

        Vector3 targetPosition = enemy.Target.transform.position;
        targetPosition.y = enemy.transform.position.y; // Y축 고정
        enemy.transform.LookAt(targetPosition);
        enemy.CurrentMoveDirection = _prevDirection;
    }
}
