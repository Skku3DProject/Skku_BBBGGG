using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private WeaponAttackBase _currentWeaponAttack;
    public WeaponAttackBase CurrentWeaponAttack => _currentWeaponAttack;

    void Start()
    {
        SwitchWeaponAttack();
        PlayerEquipmentController.Instance.OnChangeEquipment += SwitchWeaponAttack;
    }

    void Update()
    {
        if (PlayerModeManager.Instance.CurrentMode != EPlayerMode.Weapon) return;

        _currentWeaponAttack?.Tick();
    }

    public void SwitchWeaponAttack()
    {
        var weaponType = PlayerEquipmentController.Instance.GetCurrentEquipType();

        switch (weaponType)
        {
            case EquipmentType.Sword:
                _currentWeaponAttack = GetComponent<SwordAttack>();
                //_currentWeaponAttack.IsAttacking = false;
                break;

            case EquipmentType.Bow:
                _currentWeaponAttack = GetComponent<BowAttack>();
                //_currentWeaponAttack.IsAttacking = false;
                break;

            default:
                _currentWeaponAttack = GetComponent<SwordAttack>();
                //_currentWeaponAttack.IsAttacking = false;
                break;
        }
    }

    public void TryDamageEnemy(GameObject enemy, Vector3 dir)
    {
        _currentWeaponAttack?.TryDamageEnemy(enemy, dir);
    }
}
