using System;
using UnityEngine;

public enum SkillType
{
    Sword,
    Bow,
    Magic
}
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public SkillTreeMaker[] SkillTreeMakers;

    public SkillType TreeType;
    // 스킬 트리 생성
    private SkillTree _swordSkillTree;
    private SkillTree _bowSkillTree;
    private SkillTree _magicSkillTree;

    public int SkillPoint { get; private set; }

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
        
    }

    private void OnEnable()
    {
        SkillPoint = 10;
    }

    private void Start()
    {
        _swordSkillTree = SkillTreeMakers[0].Skill;
        // _bowSkillTree = SkillTreeMakers[1].Skill;
        // _magicSkillTree = SkillTreeMakers[2].Skill;
        
    }
    // 스킬 포인트 생성
    public void GainSkillPoint(int amount)
    {
        SkillPoint += amount;
    }
    
    // 스킬 찾아서 맥스레벨 체크하기
    public bool MaxLevelCheck(SkillTree tree, string skillName)
    {
        SkillNode node = tree.FindSkill(skillName);
        return node.IsMaxLevel;
    }
    // 스킬 레벨업 하기
    public bool OnClickLevelUp(SkillType type, string skillName)
    {
        SkillTree tree = null;
        
        switch(type)
        {
            case SkillType.Sword:
                tree = _swordSkillTree;
                break;
            case SkillType.Bow:
                tree = _bowSkillTree;
                break;
            case SkillType.Magic:
                tree = _magicSkillTree;
                break;
        };
        
        if (tree == null)
        {
            Debug.LogError("SkillTree is null.");
            return false;
        }
        
        Debug.Log(skillName);
        // 스킬 포인트 체크
        if (SkillPoint <= 0)
        {
            Debug.Log("Skill Point is 0");
            return false;   
        }
        // 스킬 레벨업 요소가 충족 되었는가?
        if (tree.LevelUpSkill(skillName))
        {
            SkillPoint--;
            Debug.Log(SkillPoint);
        }
        else
        {
            Debug.Log("Skill Not Found");
            return false;
        }
        
        return MaxLevelCheck(tree, skillName);
    }
}
