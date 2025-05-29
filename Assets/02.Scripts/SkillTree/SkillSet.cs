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
    private bool _open = false;
   
    private void Start()
    {
        skillButton = GetComponent<Button>(); 
        SkillManager.instance.RegisterSkill(this);
    }
    public void OnClickAppearLevelUp()
    {
        _open = !_open;
        ChildButton.gameObject.SetActive(_open);
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
            _open = false;
        }
        
    }

    public void SkillLineOn()
    {
        LineOn.gameObject.SetActive(true);
        LineOff.gameObject.SetActive(false);
    }
}
