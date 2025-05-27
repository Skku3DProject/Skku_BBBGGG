using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TempSkillSlot : MonoBehaviour
{
    public SkillCooltimeHandler CooltimeHandler;
    public SO_TempSkillSlot SlotData; // 데이터 받아오는 SO
    private SkillNode _skillNode;
    [SerializeField]private SkillTory _skillTory;
    public Image SkillIcon;       //  sprite -> 아이콘
    public float Cooldown;        // 적용할 쿨타임 (여기서 독립적으로 관리)

    public bool IsCooldown => Cooltimer <= 0; // 쿨타임 다 돌았나?
    public bool IsActive = true; // 스킬 사용할 수 있나요?
    
    // 변경되는 스킬 아이콘들 = 스킬이 쿨타임이면 변한다.
    public Image CooltimeSlider ;    // fillAmount 사용
    public float Cooltimer = 0;      // 쿨타임용 타이머
    
    public Image LockedImage;
    private void OnEnable()
    {
        _skillTory = GetComponentInParent<SkillTory>();
        _skillNode = FindSkills(SlotData.Name);
        Cooldown = _skillNode.Cooldown;
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

    public void UseSkill()
    {
        if (!IsActive)
        {
         return;   
        }

        if (IsCooldown)
        {
            
            StartCoroutine(UseSkillCoroutine());   
        }
    }
    // 스킬 사용하면 쿨타임 적용하기
    private IEnumerator UseSkillCoroutine()
    {
        Cooltimer = Cooldown;
        
        while (Cooltimer > 0)
        {
            Cooltimer -= Time.deltaTime;
            CoolTimerOn(Cooltimer);
            yield return null;
        }
    }

    private void CoolTimerOn(float cooltime)
    {
        CooltimeSlider.fillAmount = cooltime / Cooldown;
    }

    private SkillNode FindSkills(string skillName)
    {
        Debug.Log(_skillTory);
        
        SkillTree tree = SkillManager.instance.TreeCheck(_skillTory.InvenType);

        return tree.FindSkill(skillName);
    }
}
