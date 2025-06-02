using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

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
    public SkillTory[] SkillTories;
    public SkillType SkillType;

    // 스킬 트리 생성
    public SkillTree _swordSkillTree;
    public SkillTree _bowSkillTree;
    public SkillTree _magicSkillTree;

    private Dictionary<string, SkillSet> _skillSetDict = new Dictionary<string, SkillSet>();
    public Action skillAction;
    public SkillTory SelectskillTory { get; private set; }

    [SerializeField] private int _skillPoint => PlayerRewardManager.Instance.skillPoints;

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

        foreach (var maker in SkillTreeMakers)
        {
            maker.InitTree();
        }
        
        // 트리 세팅
        _swordSkillTree = SkillTreeMakers[0].Skill;
        _bowSkillTree = SkillTreeMakers[1].Skill;
        _magicSkillTree = SkillTreeMakers[2].Skill;
    }

    private void Start()
    {
        // 스킬 창 세팅
        SelectskillTory = SkillTories[0];
        SelectskillTory.gameObject.SetActive(true);

    }
    
    // skillset children을 찾기 위한 skillset구하기
    public void RegisterSkill(SkillSet skillSet)
    {
        if (!_skillSetDict.ContainsKey(skillSet.skillNode.Name))
        {
            _skillSetDict.Add(skillSet.skillNode.Name, skillSet);
        }
    }
    // 이름을 가지고 skill 
    public void SetChildrenInteractable(string skillName, bool interactable)
    {
        SkillTree tree = TreeCheck(_skillSetDict[skillName].skillNode.Type);
        SkillNode parentNode = tree.FindSkill(skillName);

        if (parentNode == null) return;

        foreach(var childNode in parentNode.Children)
        {
            if(_skillSetDict.TryGetValue(childNode.Name, out SkillSet childSkillSet))
            {
                childSkillSet.skillButton.interactable = interactable;
                childSkillSet.SkillLineOn();
            }
        }
    }
    public bool CanLevelUp(SkillType type, string skillName)
    {
        SkillTree tree = TreeCheck(type);
        SkillNode node = tree.FindSkill(skillName);
        return node.IsUnlocked;
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
        SkillTree tree = TreeCheck(type);
        if (tree == null)
        {
            return false;
        }
        
        // 스킬 포인트 체크
        if (_skillPoint <= 0)
        {
            return false;   
        }
        // 스킬 레벨업 요소가 충족 되었는가?
        if (tree.LevelUpSkill(skillName))
        {
            PlayerRewardManager.Instance.UseSkillPoint();
            SetChildrenInteractable(skillName, true);
        }
        else
        {
            return false;
        }
        // 맥스레벨에 달성하였는가?
        return MaxLevelCheck(tree, skillName);
    }

    public SkillTree TreeCheck(SkillType type)
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
        
        return tree;
    }

    public void SwitchSkilltory(EquipmentType type)
    {
        SelectskillTory.gameObject.SetActive(false);
        
        switch(type)
        {
            case EquipmentType.Sword:
                SelectskillTory = SkillTories[0];
                break;
            case EquipmentType.Bow:
                SelectskillTory = SkillTories[1];
                break;
            case EquipmentType.Magic:
                SelectskillTory = SkillTories[2];
                break;
        }
        
        SelectskillTory.gameObject.SetActive(true);
        
    }

    public bool CanUseSkill(int index)
    {
        return SelectskillTory.SkillSlots[index].IsActive;
    }

}
