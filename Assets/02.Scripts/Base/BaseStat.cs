using System;
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

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (StageManager.instance.CurrentPhase != EPhaseType.Combat)
        { 
            UIManager.instance.UI_Interaction("전투 시작하기");   
        }
        else
        {
            UIManager.instance.DiscriptionObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            StageManager.instance.CombatStart();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return; 
        }
        
        UIManager.instance.DiscriptionObject.SetActive(false);
    }
}
