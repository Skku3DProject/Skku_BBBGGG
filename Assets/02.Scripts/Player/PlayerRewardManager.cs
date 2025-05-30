using UnityEngine;

public class PlayerRewardManager : MonoBehaviour
{
    public static PlayerRewardManager Instance { get; private set; }

    public int skillPoints { get; private set; } = 0;
    public int potionCount { get; private set; } = 0;
    public bool hasBlessingBuff { get; private set; } = false;  // 가호 버프

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        StageManager.instance.OnCombatEnd += AddSkillPointAfterCombat;
    }
    public void AddSkillPoint(int amount = 1)
    {
        skillPoints += amount;
        Debug.Log($"스킬 포인트 +{amount} (총: {skillPoints})");
        // UI 갱신 등 추가 처리
        UIManager.instance.UI_SkillPointRefresh(skillPoints);
    }
    public void AddSkillPointAfterCombat()
    {
        skillPoints += 2;
        // UI 갱신 등 추가 처리
        UIManager.instance.UI_SkillPointRefresh(skillPoints);
    }
    public void AddPotion(int amount = 1)
    {
        potionCount += amount;
        Debug.Log($"물약 +{amount} (총: {potionCount})");
        // 인벤토리 연동 등
    }

    public void ApplyBlessingBuff()
    {
        if (!hasBlessingBuff)
        {
            hasBlessingBuff = true;
            Debug.Log("가호 버프 획득! (영구 지속)");
            // 예: 방어력 증가, 체력 회복량 증가 등 효과 적용
        }
        else
        {
            Debug.Log("이미 가호를 보유 중입니다.");
        }
    }

    public void UseSkillPoint() => skillPoints -= 1;

    public bool UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            Debug.Log($"물약 사용! (남은: {potionCount})");
            // TODO: 체력 회복 등 실제 효과 추가
            return true;
        }
        else
        {
            Debug.Log("물약이 없습니다.");
            return false;
        }
    }
}
