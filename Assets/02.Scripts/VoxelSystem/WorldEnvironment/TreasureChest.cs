using UnityEngine;
public enum RewardType
{
    Buff,
    Potion,
    SkillPoint
}
public class TreasureChest : MonoBehaviour
{
    public Transform lid;                // �Ѳ� ������Ʈ
    public float openAngle = 100f;       // ���� ����
    public float openSpeed = 2f;         // ������ �ӵ�
    private bool isOpen = false;         // ���� ���� ����
    private Quaternion closedRot;        // ���� ȸ����
    private Quaternion openRot;          // ���� ȸ����
    private bool hasGivenReward = false; // ������ �� ���� ����

    void Start()
    {
        // �ʱ� ȸ���� ����
        closedRot = lid.localRotation;
        openRot = Quaternion.Euler(openAngle, 0f, 0f); // X�� �������� ȸ��
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
        if (isOpen) return; // �̹� ���� ��� �ٽ� ������ ����
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
            UIManager.instance.UI_Interaction("���� �����ϱ�");
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
