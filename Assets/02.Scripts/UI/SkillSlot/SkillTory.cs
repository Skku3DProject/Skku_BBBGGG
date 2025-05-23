using System;
using UnityEngine;
public class SkillTory : MonoBehaviour
{
    public TempSkillSlot[] SkillSlots;
    public SkillType InvenType;
    
    private void Start()
    {
        SetSkills();
    }
    // 스킬 아이콘, 데이터 정보 변경
    public void SetSkills()
    {
        foreach (var skill in SkillSlots)
        {
            skill.Setup();
        }
    }

    public SkillNode FindSkillData(string skillName)
    {
        SkillTree tree = SkillManager.instance.TreeCheck(this.InvenType);
        return tree.FindSkill(skillName);
    }
}
