using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TempSkillSlot : MonoBehaviour
{
    public SO_TempSkillSlot SlotData; // 데이터 받아오는 SO
    private SkillNode _skillNode;
    [SerializeField]private SkillTory _skillTory;
    public Image SkillIcon;       //  sprite -> 아이콘
    public bool IsActive = true; // 스킬 사용할 수 있나요?
    
    // 변경되는 스킬 아이콘들 = 스킬이 쿨타임이면 변한다.
    public Image LockedImage;

    private void OnEnable()
    {
        _skillTory = GetComponentInParent<SkillTory>();
        _skillNode = FindSkills(SlotData.Name);
        ActivateCheck();
        _skillNode.SkillUnlockedAction += ActivateSkillSlot;
    }
    // 스킬 슬롯창에 들어갈 아이콘 설정
    public void Setup()
    {
        SkillIcon.sprite = _skillNode.Icon;
        IsActive = _skillNode.IsActive;
    }

    private void ActivateCheck()
    { 
        if (_skillNode.IsActive)
        {
            ActivateSkillSlot();
        }
    }
    private void ActivateSkillSlot()
    {
        IsActive = true;
        LockedImage.gameObject.SetActive(false);
    }
    
    private SkillNode FindSkills(string skillName)
    {
        SkillTree tree = SkillManager.instance.TreeCheck(_skillTory.InvenType);

        return tree.FindSkill(skillName);
    }
}
