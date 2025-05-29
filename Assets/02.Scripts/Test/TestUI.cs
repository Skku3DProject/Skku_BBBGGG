using System;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    private EquipmentType _equipmentType;
    public SkillCooltimeHandler cooltimeHandler;
    public float Timer;
    
    private void Start()
    {
        StageManager.instance.OnCombatEnd += ActionTest;
        StageManager.instance.OnCombatStart += CombatTest;
    }
    void Update()
    {
        SkillSwitchingTest();
        CooltimeTset();
    }
    private void SkillSwitchingTest()
    {
        // if(Input.GetKeyDown(KeyCode.Alpha1)) // 검
        // {
        //     _equipmentType = EquipmentType.Sword;
        //     SkillManager.instance.SwitchSkilltory(_equipmentType);
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha2))//활
        // {
        //     _equipmentType = EquipmentType.Bow;
        //     SkillManager.instance.SwitchSkilltory(_equipmentType);
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha3)) //지팡이
        // {
        //     _equipmentType = EquipmentType.Magic;
        //     SkillManager.instance.SwitchSkilltory(_equipmentType);
        // }
    }
    public void CombatTest()
    {
    }
    public void ActionTest()
    {
    }
    // 2. 그 스킬칸의 isactive를 체크
    /// <summary>
    /// 스킬 사용시 -> 스킬을 찾아서 -> 스킬의 쿨타임을 적용시킨다.
    /// 1. 스킬을 사용한다. -> input값에 따라 skilltory안에 slot의 n번째 스킬을 체크함
    /// 2. 그 스킬칸의 isactive를 체크
    /// 3. 만약 true라면 -> use스킬을 사용한다.
    /// 4. 
    /// </summary>
    public void CooltimeTset()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            cooltimeHandler.StartCooldown(SkillManager.instance.SelectskillTory.SkillSlots[0]);
            
        }
    }

    public void TutorialTest()
    {

    }
    
}
