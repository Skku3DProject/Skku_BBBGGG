using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TempSkillSlot : MonoBehaviour
{
    public SO_TempSkillSlot SlotData; // 데이터 받아오는 SO
    private SkillNode _skillNode;
    [SerializeField]private SkillTory _skillTory;
    public Image SkillIcon;       //  sprite -> 아이콘
    public float Cooldown;        // 적용할 쿨타임 (여기서 독립적으로 관리)

    private bool _isCooldown => Cooldown <= 0; // 쿨타임 다 돌았나?
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
        _skillNode.SkillUnlockedAction += ActivateSkillSlot;
        Debug.Log("temp skill start");
    }
    // 스킬 슬롯창에 들어갈 아이콘 설정
    public void Setup()
    {
        SkillIcon.sprite = _skillNode.Icon;
        IsActive = _skillNode.IsActive;
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

        if (_isCooldown)
        {
            StartCoroutine(UseSkillCoroutine());   
        }
    }
    // 스킬 사용하면 쿨타임 적용하기
    private IEnumerator UseSkillCoroutine()
    {
        Cooltimer = Cooldown;
        
        while (Cooltimer <= 0)
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
        Debug.Log(_skillTory.InvenType);
        Debug.Log(skillName);
        
        Debug.Log(tree);
        
        Debug.Log("findSkilldat skill start");
        return tree.FindSkill(skillName);
    }
}
