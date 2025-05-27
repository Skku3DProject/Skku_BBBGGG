using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SkillCooltimeHandler : MonoBehaviour
{
    // 쿨타임 돌리기
    public void StartCooldown(TempSkillSlot slot)
    {
        if (!slot.IsActive)
        {
            return;
        }

        if (slot.IsCooldown)
        {
            StartCoroutine(UseSkillCoroutine(slot));
        }
    }


    private IEnumerator UseSkillCoroutine(TempSkillSlot slot)
    {
        float timer = slot.Cooldown;
        
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            slot.Cooltimer = timer;
            CoolTimerOn(slot, timer);
            yield return null;
        }
        
    }
    private void CoolTimerOn(TempSkillSlot slot, float cooltime)
    {
        slot.CooltimeSlider.fillAmount = cooltime / slot.Cooldown;
    }
}
