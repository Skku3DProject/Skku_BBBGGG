using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerRewardManager : MonoBehaviour
{
    public static PlayerRewardManager Instance { get; private set; }
    private ThirdPersonPlayer _player; // ĳ�� ����

    public int skillPoints { get; private set; } = 110;
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

        UIManager.instance.UI_SkillPointRefresh(skillPoints);

        // ���� �� �÷��̾� ĳ��
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.GetComponent<ThirdPersonPlayer>();
        }
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
                buffName = "�̵� �ӵ�";
                break;
            case BuffType.Defense:
                buffAmount = 3f;
                _player.BuffDefense += buffAmount;
                buffName = "����";
                break;
            case BuffType.Damage:
                buffAmount = 5f;
                _player.BuffDamage += buffAmount;
                buffName = "���ݷ�";
                break;
        }

        UIManager.instance.UI_BuffRefresh(selected, buffAmount); // enum ��� UI ȣ��

        return buffName; // ���� string ��ȯ ����
    }

    public void UseSkillPoint() => skillPoints -= 1;

    public bool UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            Debug.Log($"���� ���! (����: {potionCount})");
            // TODO: ü�� ȸ�� �� ���� ȿ�� �߰�
            UIManager.instance.UI_PotionCountRefresh(potionCount);
            return true;
        }
        else
        {
            Debug.Log("������ �����ϴ�.");
            return false;
        }
    }
}
