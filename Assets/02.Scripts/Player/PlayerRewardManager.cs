using UnityEngine;

public class PlayerRewardManager : MonoBehaviour
{
    public static PlayerRewardManager Instance { get; private set; }

    public int skillPoints { get; private set; } = 0;
    public int potionCount { get; private set; } = 0;
    public bool hasBlessingBuff { get; private set; } = false;  // ��ȣ ����

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
        Debug.Log($"��ų ����Ʈ +{amount} (��: {skillPoints})");
        // UI ���� �� �߰� ó��
        UIManager.instance.UI_SkillPointRefresh(skillPoints);
    }
    public void AddSkillPointAfterCombat()
    {
        skillPoints += 2;
        // UI ���� �� �߰� ó��
        UIManager.instance.UI_SkillPointRefresh(skillPoints);
    }
    public void AddPotion(int amount = 1)
    {
        potionCount += amount;
        Debug.Log($"���� +{amount} (��: {potionCount})");
        // �κ��丮 ���� ��
    }

    public void ApplyBlessingBuff()
    {
        if (!hasBlessingBuff)
        {
            hasBlessingBuff = true;
            Debug.Log("��ȣ ���� ȹ��! (���� ����)");
            // ��: ���� ����, ü�� ȸ���� ���� �� ȿ�� ����
        }
        else
        {
            Debug.Log("�̹� ��ȣ�� ���� ���Դϴ�.");
        }
    }

    public void UseSkillPoint() => skillPoints -= 1;

    public bool UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            Debug.Log($"���� ���! (����: {potionCount})");
            // TODO: ü�� ȸ�� �� ���� ȿ�� �߰�
            return true;
        }
        else
        {
            Debug.Log("������ �����ϴ�.");
            return false;
        }
    }
}
