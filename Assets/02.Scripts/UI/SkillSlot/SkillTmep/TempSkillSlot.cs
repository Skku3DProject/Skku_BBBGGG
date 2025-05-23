using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TempSkillSlot : MonoBehaviour
{
    public SO_TempSkillSlot SlotData; // 데이터 받아오는 SO
    private SkillNode _skillNode;
    private SkillTory _skillTory;
    public Image SkillIcon;       //  sprite -> 아이콘
    public float Cooldown;        // 적용할 쿨타임 (여기서 독립적으로 관리)
    
    public bool IsActive = true; // 스킬 사용할 수 있나요?
    
    // 변경되는 스킬 아이콘들 = 스킬이 쿨타임이면 변한다.
    public Image CooltimeSlider ;    // fillAmount 사용
    public float Cooltimer = 0;      // 쿨타임용 타이머
    
    public Image LockedImage;
    private void OnEnable()
    {
        _skillTory = GetComponentInParent<SkillTory>();
        _skillNode = _skillTory.FindSkillData(SlotData.name);
        _skillNode.SkillUnlockedAction += ActivateSkillSlot;
    }
    // 스킬 슬롯창에 들어갈 아이콘 설정
    public void Setup()
    {
        SkillIcon.sprite = _skillNode.Icon;
        IsActive = _skillNode.IsActive;
        
    }
    
    public void ActivateSkillSlot()
    {
        IsActive = true;
        LockedImage.gameObject.SetActive(false);
    }
    public void NodeSetup()
    {
        
    }
    
}
