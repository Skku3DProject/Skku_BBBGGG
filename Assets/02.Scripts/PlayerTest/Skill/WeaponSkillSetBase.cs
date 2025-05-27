using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSkillSetBase : MonoBehaviour
{
    //무기별로 스킬이 2개 이상 존재하므로
    [SerializeField] protected List<WeaponSkillBase> skillList;
    //휘두르기, 찌르기 등의 스킬이 여기에 들어감

    public virtual void UseSkill(int index)
    {//특정 인덱스의 스킬을 사용하고 싶을 떄

        if(index >= 0 && index < skillList.Count)
        {
            if (skillList[index].IsSkillAvailable())
            {
                skillList[index].UseSkill();
            }        
            //스킬이 사용가능한 상태이면 스킬 발동
        }
    }

    public virtual void Tick()
    {
        foreach(var skill in skillList)
        {
            skill.Tick();
            //등록된 모든 스킬에 대해 Tick()을 호출
        }
    }

    public WeaponSkillBase GetSkill(int index)
    {
        //특정 인덱스의 스킬을 외부에서 참조하고 싶을 떄
        if(index >= 0 && index < skillList.Count)
        {
            return skillList[index];
        }

        else
        {
            return null;
        }
    }
}
