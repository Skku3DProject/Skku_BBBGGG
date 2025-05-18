using System;
using UnityEngine;

public class SkillTreeMaker : MonoBehaviour
{
    public SkillTree Skill;
    public SkillSet[] SkillPrefab;

    private void Awake()
    {
        // 첫 번째 프리펩을 루트로 지정
        Skill = new SkillTree(SkillPrefab[0].skillNode);
        TreeSetting();
        Debug.Log(Skill.StartNode.Children.Count);
    }
    // 스킬들 children으로 집어넣기
    private void TreeSetting()
    {
        SkillNode set = Skill.StartNode;
        Debug.Log(set.Name);
        // 반복문 진행하면서 노드 세팅하기   
        for(int i = 1; i < SkillPrefab.Length; i++)
        {
            SkillNode node = new SkillNode(SkillPrefab[i].skillNode);
            set.AddChild(node);
            set = node;
            Debug.Log(set.Name);
        }
    }
}
