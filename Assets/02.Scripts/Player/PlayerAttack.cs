using Unity.Cinemachine;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator _playerAnimation;
    private PlayerEquipmentController _equipmentController;

    public bool IsAttacking = false;

    [Header("Attack Effects")]
    public ParticleSystem SwordSlash1Effect;
    public ParticleSystem SwordSlash2Effect;


   // [Header("Attack Settings")]
    //public PlayerStatsSO PlayerStats;

    private int _currentAttackIndex; // 현재 공격 인덱스 기억용

    void Start()
    {
        _playerAnimation = GetComponent<Animator>();
        _equipmentController = GetComponent<PlayerEquipmentController>();
    }

    void Update()
    {
        if (PlayerModeManager.Instance.CurrentMode != EPlayerMode.Weapon) return;

        if (Input.GetMouseButtonDown(0) && !IsAttacking)
        {
            if (_equipmentController.GetCurrentEquipType() == EquipmentType.Sword)
            {
                _currentAttackIndex = Random.Range(0, 2);

                if (_currentAttackIndex == 0)
                {
                    _playerAnimation.SetTrigger("SwordAttack1");
                    //Debug.Log("1번 공격");
                }
                else
                {
                    _playerAnimation.SetTrigger("SwordAttack2");
                   // Debug.Log("2번 공격");
                }

                IsAttacking = true;
            }
        }
    }

    //검이 적과 충돌
     public void TryDamageEnemy(GameObject enemy)
     {
       // Debug.Log("적과 충돌해서 공격할거임");

         if(!IsAttacking)
         {
             return;
         }
        // 현재 무기 타입 확인
        var currentEquipType = _equipmentController.GetCurrentEquipType();
        //Debug.Log($"현재 장착 무기 타입: {currentEquipType}");

        // 공격력 확인
        float attackPower = _equipmentController.GetCurrentWeaponAttackPower();
       // Debug.Log($"현재 무기 공격력: {attackPower}");

        // 데미지 줄 수 있는 대상인지 확인
        IDamageAble damageable = enemy.GetComponent<IDamageAble>();
        if (damageable != null)
        {
            Damage damage = new Damage(attackPower, gameObject, 3f);
            damageable.TakeDamage(damage);
           // Debug.Log($"공격 성공: {enemy.name}에게 {attackPower} 데미지를 줌");
        }
        else
        {
            Debug.LogWarning($"IDamageAble 컴포넌트를 {enemy.name}에서 찾을 수 없음");
        }
    }






    // 애니메이션 이벤트에서 호출됨
    public void OnAttackEffectPlay()
    {
        //검 공격 시 이펙트
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
        IsAttacking = false;
    }


}
