using UnityEngine;

public interface IEnemyMoveTypeStrategy 
{
    public void Move(Enemy enemy, float speed);
}
