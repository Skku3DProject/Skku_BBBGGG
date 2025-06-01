using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SkillCooltimeHandler : MonoBehaviour
{
    private TempSkillSlot _currentSkillSlot;
    private bool _isPlayerSkill = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentSkillSlot = SkillManager.instance.SelectskillTory.SkillSlots[0];
            StartCooldown(_currentSkillSlot);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _currentSkillSlot = SkillManager.instance.SelectskillTory.SkillSlots[1];
            StartCooldown(_currentSkillSlot);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentSkillSlot = SkillManager.instance.SelectskillTory.SkillSlots[2];
            StartCooldown(_currentSkillSlot);
        }
    }

    // 쿨타임 돌리기
    public void StartCooldown(TempSkillSlot slot)
    {
        if (!slot.IsActive)
        {
            return;
        }
        Debug.Log("IsActive");
        if (!slot.Skill.IsUsingSkill)
        {
            return;
        }
        Debug.Log("IsUsingSkill");
        if (slot.IsCooldown)
        {
            slot.Skill.UseSkill();
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
