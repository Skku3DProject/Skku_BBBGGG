using System;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public SkillTreeMaker[] SkillTreeMakers;
    // 스킬 트리 생성
    private SkillTree _swordSkillTree;
    private SkillTree _bowSkillTree;
    private SkillTree _magicSkillTree;

    public int SkillPoint = 20;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        // _bowSkillTree = SkillTreeMakers[1].Skill;
        // _magicSkillTree = SkillTreeMakers[2].Skill;
        
    }

    private void Start()
    {
        _swordSkillTree = SkillTreeMakers[0].Skill;
        if (_swordSkillTree == null)
        {
            Debug.Log("Skill Tree is null");
        }
    }

    public void OnClickLevelUp(string skillName)
    {
        Debug.Log(skillName);
        if (_swordSkillTree.LevelUpSkill(skillName))
        {
            SkillPoint--;
            Debug.Log(SkillPoint);
        }
        else
        {
            Debug.Log("Skill Not Found");
            return;
        }

        ;
    }
}
