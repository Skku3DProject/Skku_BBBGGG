using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //�÷��̾��� ���� ��ǿ� ���� ��Ƴ��� ��
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;

    public void Start()
    {
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
    }

    //�� - ����
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))//���� ���콺 Ŭ���ϸ�
        {
            if (_equipmentController.GetCurrentEquipType() == EquipmentType.Sword)
            {
                //�������� 2�� ���� ��� �߿� �ϳ��� ����ȴ�.
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    _playerAnimation.SetTrigger("SwordAttack1");
                    Debug.Log("1�� ����");
                }

                else
                {
                    _playerAnimation.SetTrigger("SwordAttack2");
                    Debug.Log("2�� ����");
                }
            }

                
        }
    }
}
