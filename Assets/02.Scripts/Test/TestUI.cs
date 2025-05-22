using System;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    private EquipmentType _equipmentType;
    private void Start()
    {
        StageManager.instance.OnCombatEnd += ActionTest;
        StageManager.instance.OnCombatStart += CombatTest;
    }
    void Update()
    {
        SkillSwitchingTest();
    }
    private void SkillSwitchingTest()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) // 검
        {
            _equipmentType = EquipmentType.Sword;
            SkillManager.instance.SwitchSkilltory(_equipmentType);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))//활
        {
            _equipmentType = EquipmentType.Bow;
            SkillManager.instance.SwitchSkilltory(_equipmentType);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) //지팡이
        {
            _equipmentType = EquipmentType.Magic;
            SkillManager.instance.SwitchSkilltory(_equipmentType);
        }
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
