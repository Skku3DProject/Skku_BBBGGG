using System.Collections.Generic;
using UnityEngine;

public class SkillNode
{
    private SkillNodeSO _data;
    // 데이터 고정 값
    public string Name => _data.Name;
    public int MaxLevel => _data.MaxLevel;
    public List<SkillNode> Children {get; private set;} // 자식 스킬 목록
    public SkillNode Parent {get; private set;} // 선행 스킬 -> 먼저 필요한 스킬
    
    // 변하는 데이터
    // 스킬 레벨 
    public int SkillLevel { get; private set; } = 0; //시작 레벨은 항상 0
    public bool IsUnlocked => Parent == null || Parent.SkillLevel > 0; // 부모 스킬이 찍혔는가
    public bool IsActive => SkillLevel > 0;   // 현재 사용하려는 스킬이 찍혔는가
    
    public SkillNode(SkillNodeSO data)
    {
        _data = data;
        Children = new List<SkillNode>();
    }
    // 스킬 자식으로 넣기 -> 리스트에 추가한다.
    public void AddChild(SkillNode child)
    {
        Children.Add(child);
        child.Parent = this;
    }
    // 스킬 레벨 올리기 => 부모가 없거나, 부모의 레벨이 0 보다 높거나 이 스킬의 레벨이 맥스 레벨 보다 낮으면 스킬 찍을 수 있음
    public bool LevelUp()
    {
        if (IsUnlocked && SkillLevel < MaxLevel )
        {
            SkillLevel++;
            Debug.Log(SkillLevel);
            return true;
        }
        return false;
    }
}
