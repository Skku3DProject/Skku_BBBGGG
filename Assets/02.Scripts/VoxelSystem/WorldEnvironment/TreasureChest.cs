using UnityEngine;
public enum RewardType
{
    Buff,
    Potion,
    SkillPoint
}
public enum BuffType
{
    Speed,
    Defense,
    Damage
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


    private bool isRemoved = false; // �ߺ� ���� ������
    [Header("����Ʈ")]
    public GameObject destroyEffect;
    public GameObject LoopEffect;
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

            // �Ѳ��� ���� �� ���ȴ��� Ȯ�� (���� ���� ��)
            if (!isRemoved && Quaternion.Angle(lid.localRotation, openRot) < 1f)
            {
                isRemoved = true;
                Invoke(nameof(RemoveChest), 1f); // ���� �Ϸ� �� 1�� �� ����
            }
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
        HandleReward(RewardType.Buff);
    }

    void HandleReward(RewardType reward)
    {
        var rewardPopup = PopUpManager.Instance.OpenPopup<UI_RewardPopup>(EPopupType.UI_RewardPopup);

        switch (reward)
        {
            case RewardType.Buff:
                string buffName = PlayerRewardManager.Instance.ApplyBlessingBuff();
                rewardPopup.ShowReward(reward, buffName); // ���� �̸� ����
                break;

            case RewardType.Potion:
                PlayerRewardManager.Instance.AddPotion();
                rewardPopup.ShowReward(reward); // ������ ����
                break;

            case RewardType.SkillPoint:
                PlayerRewardManager.Instance.AddSkillPoint();
                rewardPopup.ShowReward(reward); // ������ ����
                break;
        }
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            UIManager.instance.UI_Interaction(InteractionType.Chest);
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

void RemoveChest()
{
    if (destroyEffect != null)
    {
       Instantiate(destroyEffect, transform.position, Quaternion.identity);

    }

    Destroy(gameObject);
}
}
