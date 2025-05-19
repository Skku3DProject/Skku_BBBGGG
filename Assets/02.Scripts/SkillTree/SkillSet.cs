using System;
using UnityEngine;
using UnityEngine.UI;


public class SkillSet : MonoBehaviour
{
    public SkillNodeSO skillNode;
    public Button skillButton;

    private void Start()
    {
        skillButton = this.GetComponent<Button>();
        skillButton.interactable = SkillManager.instance.CanLevelUp(skillNode.Type, skillNode.Name);
    }
    // 버튼 클릭으로 스킬 레벨 올리기.
    public void OnClickLevelUp()
    {
        if (SkillManager.instance.OnClickLevelUp(skillNode.Type, skillNode.Name))
        {
            skillButton.interactable = false;
        }
        else
        {
            skillButton.interactable = true;
        }
    }
}
