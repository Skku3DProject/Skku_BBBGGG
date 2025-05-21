using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;

    private bool _isAttacking = false;

    [Header("Attack Effects")]
    public ParticleSystem SwordSlash1Effect;
    public ParticleSystem SwordSlash2Effect;

    private int _currentAttackIndex; // ���� ���� �ε��� ����

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
                _currentAttackIndex = Random.Range(0, 2);

                if (_currentAttackIndex == 0)
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

    // �ִϸ��̼� �̺�Ʈ���� ȣ���
    public void OnAttackEffectPlay()
    {
        if (_currentAttackIndex == 0 && SwordSlash1Effect != null)
        {
            SwordSlash1Effect.Play();
        }
        else if (_currentAttackIndex == 1 && SwordSlash2Effect != null)
        {
            SwordSlash2Effect.Play();
        }
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ��� (������ ������ ��ó)
    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
    }
}
