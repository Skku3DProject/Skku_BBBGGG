using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //플레이어의 공격 모션에 대해 모아놓은 곳
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;

    public void Start()
    {
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
    }

    //검 - 공격
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))//왼쪽 마우스 클릭하면
        {
            if (_equipmentController.GetCurrentEquipType() == EquipmentType.Sword)
            {
                //랜덤으로 2개 공격 모션 중에 하나가 실행된다.
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    _playerAnimation.SetTrigger("SwordAttack1");
                    Debug.Log("1번 공격");
                }

                else
                {
                    _playerAnimation.SetTrigger("SwordAttack2");
                    Debug.Log("2번 공격");
                }
            }

                
        }
    }
}
