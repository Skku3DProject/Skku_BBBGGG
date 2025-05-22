using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TempSkillSlot : MonoBehaviour
{
    public SO_TempSkillSlot skillNode; // 데이터 받아오는 SO
    public Image SkillIcon;       //  sprite -> 아이콘
    public float Cooldown;        // 적용할 쿨타임 (여기서 독립적으로 관리)
    
    public bool IsActive = true; // 스킬 사용할 수 있나요?
    
    // 변경되는 스킬 아이콘들 = 스킬이 쿨타임이면 변한다.
    public Image CooltimeSlider ;    // fillAmount 사용
    public float Cooltimer = 0;      // 쿨타임용 타이머

    private void OnEnable()
    {
    }
    // 무기가 바뀌면 스킬 창에 위치한 스킬들이 변경된다.
    public void Setup()
    {
        SkillIcon.sprite = this.skillNode.Icon;
        Cooldown = this.skillNode.CoolDown;
    }
    
}
