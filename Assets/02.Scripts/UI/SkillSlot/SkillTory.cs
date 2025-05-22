using System;
using UnityEngine;
public class SkillTory : MonoBehaviour
{
    public TempSkillSlot[] SkillSlots;
    
    private void Start()
    {
        SetSkills();
    }

    public void SetSkills()
    {
        foreach (var skill in SkillSlots)
        {
            skill.Setup();
        }
    }
}
