using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;

    private bool _isAttacking = false;

    void Start()
    {
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isAttacking)
        {
            if (_equipmentController.GetCurrentEquipType() == EquipmentType.Sword)
            {
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

                _isAttacking = true;
            }
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ��Ǵ� �Լ�
    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
    }
}
