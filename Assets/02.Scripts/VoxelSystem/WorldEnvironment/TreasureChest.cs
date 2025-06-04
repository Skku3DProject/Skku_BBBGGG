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
    public Transform lid;                // 뚜껑 오브젝트
    public float openAngle = 100f;       // 열릴 각도
    public float openSpeed = 2f;         // 열리는 속도
    private bool isOpen = false;         // 현재 열린 상태
    private Quaternion closedRot;        // 닫힌 회전값
    private Quaternion openRot;          // 열린 회전값
    private bool hasGivenReward = false; // 보상은 한 번만 지급


    private bool isRemoved = false; // 중복 제거 방지용
    [Header("이펙트")]
    public GameObject destroyEffect;
    public GameObject LoopEffect;
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

            // 뚜껑이 거의 다 열렸는지 확인 (각도 차이 비교)
            if (!isRemoved && Quaternion.Angle(lid.localRotation, openRot) < 1f)
            {
                isRemoved = true;
                Invoke(nameof(RemoveChest), 1f); // 열림 완료 후 1초 뒤 삭제
            }
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
        HandleReward(RewardType.Buff);
    }

    void HandleReward(RewardType reward)
    {
        var rewardPopup = PopUpManager.Instance.OpenPopup<UI_RewardPopup>(EPopupType.UI_RewardPopup);

        switch (reward)
        {
            case RewardType.Buff:
                string buffName = PlayerRewardManager.Instance.ApplyBlessingBuff();
                rewardPopup.ShowReward(reward, buffName); // 버프 이름 전달
                break;

            case RewardType.Potion:
                PlayerRewardManager.Instance.AddPotion();
                rewardPopup.ShowReward(reward); // 디테일 없음
                break;

            case RewardType.SkillPoint:
                PlayerRewardManager.Instance.AddSkillPoint();
                rewardPopup.ShowReward(reward); // 디테일 없음
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
