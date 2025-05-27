using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSkillSetBase : MonoBehaviour
{
    //���⺰�� ��ų�� 2�� �̻� �����ϹǷ�
    [SerializeField] protected List<WeaponSkillBase> skillList;
    //�ֵθ���, ��� ���� ��ų�� ���⿡ ��

    public virtual void UseSkill(int index)
    {//Ư�� �ε����� ��ų�� ����ϰ� ���� ��

        if(index >= 0 && index < skillList.Count)
        {
            if (skillList[index].IsSkillAvailable())
            {
                skillList[index].UseSkill();
            }        
            //��ų�� ��밡���� �����̸� ��ų �ߵ�
        }
    }

    public virtual void Tick()
    {
        foreach(var skill in skillList)
        {
            skill.Tick();
            //��ϵ� ��� ��ų�� ���� Tick()�� ȣ��
        }
    }

    public WeaponSkillBase GetSkill(int index)
    {
        //Ư�� �ε����� ��ų�� �ܺο��� �����ϰ� ���� ��
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
