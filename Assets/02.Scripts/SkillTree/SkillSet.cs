using System;
using UnityEngine;
using UnityEngine.UI;


public class SkillSet : MonoBehaviour
{
    public SkillNodeSO skillNode;
    public Button skillButton;
    public Image LineOn;
    public Image LineOff;
    public Button ChildButton;
    public Image Lock;
    public bool Open = false;
   
    private void Start()
    {
        skillButton = GetComponent<Button>(); 
        SkillManager.instance.RegisterSkill(this);
        Lock.gameObject.SetActive(!skillButton.interactable);
    }
    public void OnClickAppearLevelUp()
    {
        SkillTreeMaker[] skillTree = SkillManager.instance.SkillTreeMakers;
        foreach (SkillTreeMaker treeMaker in skillTree)
        {
            foreach (SkillSet skillSet in treeMaker.SkillPrefab)
            {
                skillSet.Open = false;
                skillSet.ChildButton.gameObject.SetActive(false);
            }
        }

        Open = !Open;
        ChildButton.gameObject.SetActive(Open);
    }
    
    // 버튼 클릭으로 스킬 레벨 올리기.
    public void OnClickLevelUp()
    {
        if (SkillManager.instance.OnClickLevelUp(skillNode.Type, skillNode.Name))
        {
            skillButton.interactable = false;
            ChildButton.gameObject.SetActive(false);
            
        }
        else
        {
            skillButton.interactable = true;
            
            ChildButton.gameObject.SetActive(false);
            Open = false;
        }
        
    }

    public void SkillLineOn()
    {
        LineOn.gameObject.SetActive(true);
        LineOff.gameObject.SetActive(false);
        Lock.gameObject.SetActive(!skillButton.interactable);
    }

    private void OnDisable()
    {
        Open = false;
        ChildButton.gameObject.SetActive(false);
    }
}
