using UnityEngine;

public class BaseStat : MonoBehaviour, IDamageAble
{
    public float BaseHP = 2000;
    
    public void TakeDamage(Damage damage)
    {
        BaseHP -= damage.Value;

        if (BaseHP <= 0)
        {
            GameManager.instance.ChangeState(GameState.GameOver);
        }
    }
    
    
}
