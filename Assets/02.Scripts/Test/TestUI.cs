using System;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    private void Start()
    {
        StageManager.instance.OnCombatEnd += ActionTest;
        StageManager.instance.OnCombatStart += CombatTest;
    }

    public void CombatTest()
    {
        Debug.Log("몬스터 소환");
    }
    public void ActionTest()
    {
        Debug.Log("스테이지 종료");
    }
}
