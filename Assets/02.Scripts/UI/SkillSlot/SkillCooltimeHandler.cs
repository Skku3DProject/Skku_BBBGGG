using System.Collections;
using UnityEngine;

public class SkillCooltimeHandler : MonoBehaviour
{
    // private TempSkillSlot _currentSkillSlot;
    // public void SkillMenuCooltime(WeaponSkillBase skill, float cooltime)
    // {
    //     if (skill.IsCooldown)
    //     {
    //         return;
    //     }
    //     if (Input.GetKeyDown(KeyCode.Q))
    //     {
    //         _currentSkillSlot = SkillManager.instance.SelectskillTory.SkillSlots[0];
    //         StartCooldown(_currentSkillSlot,cooltime);
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         _currentSkillSlot = SkillManager.instance.SelectskillTory.SkillSlots[1];
    //         StartCooldown(_currentSkillSlot,cooltime);
    //     }
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         _currentSkillSlot = SkillManager.instance.SelectskillTory.SkillSlots[2];
    //         StartCooldown(_currentSkillSlot,cooltime);
    //     }        
    // }
    // // 쿨타임 돌리기
    // private void StartCooldown(TempSkillSlot slot, float cooldown)
    // {
    //     if (!slot.IsActive)
    //     {
    //         return;
    //     }
    //     
    //     StartCoroutine(UseSkillCoroutine(slot, cooldown));
    // }
    // private void CoolTimerOn(TempSkillSlot slot, float cooltime)
    // {
    //     slot.CooltimeSlider.fillAmount = cooltime ;
    // }
    //
    // private IEnumerator UseSkillCoroutine(TempSkillSlot slot, float cooldown)
    // {
    //     float timer = cooldown;
    //     
    //     while (timer > 0)
    //     {
    //         timer -= Time.deltaTime;
    //         CoolTimerOn(slot, cooldown);
    //         yield return null;
    //     }
    //     
    // }
    // // private void CoolTimerOn(TempSkillSlot slot, float cooltime, float CooldownTime)
    // // {
    // //     slot.CooltimeSlider.fillAmount = cooltime / CooldownTime;
    // // }
}
