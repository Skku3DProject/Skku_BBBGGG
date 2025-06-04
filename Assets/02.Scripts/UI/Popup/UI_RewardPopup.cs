using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardPopup : UI_Popup
{
    [Header("UI ����")]
    public TextMeshProUGUI rewardText;
    public Image rewardIcon;

    [Header("������ ���ҽ�")]
    public Sprite buffIcon;
    public Sprite potionIcon;
    public Sprite skillPointIcon;

    public void ShowReward(RewardType rewardType, string detail = "", System.Action callback = null)
    {
        Open(callback);

        switch (rewardType)
        {
            case RewardType.Buff:
                rewardText.text = $"��ȣ ������ ȹ���߽��ϴ�!\n<color=#FF0000>{detail}</color> ���!";
                rewardIcon.sprite = buffIcon;
                break;

            case RewardType.Potion:
                rewardText.text = "������ ȹ���߽��ϴ�!";
                rewardIcon.sprite = potionIcon;
                break;

            case RewardType.SkillPoint:
                rewardText.text = "��ų ����Ʈ�� ȹ���߽��ϴ�!";
                rewardIcon.sprite = skillPointIcon;
                break;
        }
    }

}
