using UnityEngine;

public interface IFSM 
{
    public void Start()
    {
        
    }

    public EEnemyState Update()
    {
        return EEnemyState.Idle;
    }
    
    public void End()
    {

    }
}
