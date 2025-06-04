using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerRewardManager : MonoBehaviour
{
    public static PlayerRewardManager Instance { get; private set; }
    private ThirdPersonPlayer _player; // 캐싱 변수

    public int skillPoints { get; private set; } = 110;
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

        UIManager.instance.UI_SkillPointRefresh(skillPoints);

        // 시작 시 플레이어 캐싱
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.GetComponent<ThirdPersonPlayer>();
        }
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

    public string ApplyBlessingBuff()
    {
        if (_player == null) return "";

        int randomIndex = Random.Range(0, 3);
        string buffName = "";
        BuffType selected = (BuffType)randomIndex;
        float buffAmount = 0f;

        switch (selected)
        {
            case BuffType.Speed:
                buffAmount = 0.5f;
                _player.BuffSpeed += buffAmount;
                buffName = "이동 속도";
                break;
            case BuffType.Defense:
                buffAmount = 3f;
                _player.BuffDefense += buffAmount;
                buffName = "방어력";
                break;
            case BuffType.Damage:
                buffAmount = 5f;
                _player.BuffDamage += buffAmount;
                buffName = "공격력";
                break;
        }

        UIManager.instance.UI_BuffRefresh(selected, buffAmount); // enum 기반 UI 호출

        return buffName; // 기존 string 반환 유지
    }

    public void UseSkillPoint() => skillPoints -= 1;

    public bool UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            Debug.Log($"물약 사용! (남은: {potionCount})");
            // TODO: 체력 회복 등 실제 효과 추가
            UIManager.instance.UI_PotionCountRefresh(potionCount);
            return true;
        }
        else
        {
            Debug.Log("물약이 없습니다.");
            return false;
        }
    }
}
