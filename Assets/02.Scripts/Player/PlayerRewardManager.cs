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

    public void AddSkillPoint(int amount = 1)
    {
        skillPoints += amount;
        Debug.Log($"��ų ����Ʈ +{amount} (��: {skillPoints})");
        // UI ���� �� �߰� ó��
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
}
