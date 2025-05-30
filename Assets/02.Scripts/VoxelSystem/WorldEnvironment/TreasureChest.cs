using UnityEngine;
public enum RewardType
{
    Buff,
    Potion,
    SkillPoint
}
public class TreasureChest : MonoBehaviour
{
    public Transform lid;                // 뚜껑 오브젝트
    public float openAngle = 100f;       // 열릴 각도
    public float openSpeed = 2f;         // 열리는 속도
    private bool isOpen = false;         // 현재 열린 상태
    private Quaternion closedRot;        // 닫힌 회전값
    private Quaternion openRot;          // 열린 회전값
    private bool hasGivenReward = false; // 보상은 한 번만 지급

    void Start()
    {
        // 초기 회전값 저장
        closedRot = lid.localRotation;
        openRot = Quaternion.Euler(openAngle, 0f, 0f); // X축 기준으로 회전
    }

    void Update()
    {
        if (isOpen)
        {
            lid.localRotation = Quaternion.Slerp(lid.localRotation, openRot, Time.deltaTime * openSpeed);
        }
        else
        {
            lid.localRotation = Quaternion.Slerp(lid.localRotation, closedRot, Time.deltaTime * openSpeed);
        }
    }

    public void Interact()
    {
        if (isOpen) return; // 이미 열린 경우 다시 열리지 않음
        isOpen = true;
        GiveRandomReward();


    }
    void GiveRandomReward()
    {
        if (hasGivenReward) return;
        hasGivenReward = true;

        RewardType reward = (RewardType)Random.Range(0, System.Enum.GetValues(typeof(RewardType)).Length);
        HandleReward(reward);
    }

    void HandleReward(RewardType reward)
    {
        switch (reward)
        {
            case RewardType.Buff:
                PlayerRewardManager.Instance.ApplyBlessingBuff();
                break;
            case RewardType.Potion:
                PlayerRewardManager.Instance.AddPotion();
                break;
            case RewardType.SkillPoint:
                PlayerRewardManager.Instance.AddSkillPoint();
                break;
        }
        var rewardPopup = PopUpManager.Instance.OpenPopup<UI_RewardPopup>(EPopupType.UI_RewardPopup);
        rewardPopup.ShowReward(reward);
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            UIManager.instance.UI_Interaction("전투 시작하기");
            if (Input.GetKeyDown(KeyCode.F))
            {
                Interact();


            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        UIManager.instance.DiscriptionObject.SetActive(false);
    }
}
