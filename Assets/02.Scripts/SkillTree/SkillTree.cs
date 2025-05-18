using UnityEngine;

public class SkillTree
{
    // 스킬의 시작 지점
    public SkillNode StartNode {get; private set;}
    
    // 스킬의 시작 지점 정하기
    public SkillTree(SkillNodeSO data)
    {
        StartNode = new SkillNode(data);
    }
    // 이름을 통해 특정 스킬을 찾는다.
    public SkillNode FindSkill(string skillName)
    {
        return FindNode(StartNode, skillName);
    }
    // 스킬 레벨업을 시도한다. = 찾은 스킬이 null이면 false , null이 아니면 true
    public bool LevelUpSkill(string skillName)
    {
        SkillNode skill =  FindSkill(skillName);
        Debug.Log(skill.Name);
        return skill?.LevelUp() ?? false;
        
    }
    // 현재 노드와 자식들을 타고 내려가며 스킬을 찾는다.
    private SkillNode FindNode(SkillNode currentNode, string skillName)
    {
        
        if (currentNode.Name == skillName)
        {
            return currentNode;
        }
        foreach (var child in currentNode.Children)
        {
            var found = FindNode(child, skillName);
            if (found != null)
            {
                return found;
            }
        }
        
        return null;
    }
}
