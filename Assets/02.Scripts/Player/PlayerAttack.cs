using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;

    private bool _isAttacking = false;

    [Header("Attack Effects")]
    public ParticleSystem SwordSlash1Effect;
    public ParticleSystem SwordSlash2Effect;

    private int _currentAttackIndex; // 현재 공격 인덱스 기억용

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
                    Debug.Log("1번 공격");
                }
                else
                {
                    _playerAnimation.SetTrigger("SwordAttack2");
                    Debug.Log("2번 공격");
                }

                _isAttacking = true;
            }
        }
    }

    // 애니메이션 이벤트에서 호출됨
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

    // 애니메이션 이벤트에서 호출됨 (마지막 프레임 근처)
    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
    }
}
