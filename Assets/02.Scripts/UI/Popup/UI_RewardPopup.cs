using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardPopup : UI_Popup
{
    [Header("UI 참조")]
    public TextMeshProUGUI rewardText;
    public Image rewardIcon;

    [Header("아이콘 리소스")]
    public Sprite buffIcon;
    public Sprite potionIcon;
    public Sprite skillPointIcon;

    public void ShowReward(RewardType rewardType, string detail = "", System.Action callback = null)
    {
        Open(callback);

        switch (rewardType)
        {
            case RewardType.Buff:
                rewardText.text = $"가호 버프를 획득했습니다!\n<color=#FF0000>{detail}</color> 상승!";
                rewardIcon.sprite = buffIcon;
                break;

            case RewardType.Potion:
                rewardText.text = "물약을 획득했습니다!";
                rewardIcon.sprite = potionIcon;
                break;

            case RewardType.SkillPoint:
                rewardText.text = "스킬 포인트를 획득했습니다!";
                rewardIcon.sprite = skillPointIcon;
                break;
        }
    }

}
