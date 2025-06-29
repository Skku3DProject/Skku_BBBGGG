﻿using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private ThirdPersonPlayer _player;

    private WeaponAttackBase _currentWeaponAttack;
    public WeaponAttackBase CurrentWeaponAttack => _currentWeaponAttack;

    private WeaponSkillBase _skill1;
    public WeaponSkillBase Skill1 => _skill1;
    private WeaponSkillBase _skill2;


    public bool IsUsingJumpAnim = true;
    public bool IsMoveSlow = false;

    private EEquipmentType _equipmentType;

    public float CurrentDamage;
    private void Awake()
    {
        _player = GetComponent<ThirdPersonPlayer>();
    }
    void Start()
    {
        // 처음 기본무기는 검
        SwitchWeaponAttack(EEquipmentType.Sword);
        PlayerEquipmentManager.Instance.OnChangedEquipment += SwitchWeaponAttack;
    }

    void Update()
    {
        if (UI_TowerBuildMenu.isBuildMode) return;


        if (!_player.IsAlive || _player.IsReturning) return;

        if (PlayerModeManager.Instance.CurrentMode != EPlayerMode.Weapon) return;

        // 각 스킬들의 쿨타임이나 여러가지 업데이트할것들처리 인풋은 아래서 따로 빼서처리
        _currentWeaponAttack?.Tick();
        if(SkillManager.instance.CanUseSkill(0))
            _skill1?.Tick();
        if (SkillManager.instance.CanUseSkill(1))
            _skill2?.Tick();

        //평타 공격은 스킬이 사용중이면 안나가게
        if (Input.GetKeyDown(KeyCode.Mouse0) && _skill1.IsUsingSkill == false && _skill2.IsUsingSkill == false)
            _currentWeaponAttack?.Attack();

        //각 스킬들은 평타중에 나갈수있지만 다른 스킬사용중에는 못나가게
        if (Input.GetKeyDown(KeyCode.Q) && _skill2.IsUsingSkill == false && SkillManager.instance.CanUseSkill(0))
            _skill1?.UseSkill();
        if (Input.GetKeyDown(KeyCode.E) && _skill1.IsUsingSkill == false && SkillManager.instance.CanUseSkill(1))
            _skill2?.UseSkill();

    }

    public void SwitchWeaponAttack(EEquipmentType currentType)
    {
        _skill1?.ResetState();
        _skill2?.ResetState();

        _equipmentType = currentType;

        switch (_equipmentType)
        {
            case EEquipmentType.Sword:
                _currentWeaponAttack = GetComponent<SwordAttack>();
                _currentWeaponAttack.IsAttacking = false;
                _skill1 = GetComponent<SwordSpinSkill>();
                _skill2 = GetComponent<SwordDashSkill>();
                CurrentDamage = _player.BuffDamage + PlayerEquipmentManager.Instance.GetCurrentWeaponAttackPower();
                break;

            case EEquipmentType.Bow:
                _currentWeaponAttack = GetComponent<BowAttack>();
                _currentWeaponAttack.IsAttacking = false;
                _skill1 = GetComponent<BowFireSkill>();
                _skill2 = GetComponent<BowChargingSkill>();
                CurrentDamage = _player.BuffDamage + PlayerEquipmentManager.Instance.GetCurrentWeaponAttackPower();
                break;

            case EEquipmentType.Magic:
                _currentWeaponAttack = GetComponent<WandAttack>();
                _currentWeaponAttack.IsAttacking = false;
                _skill1 = GetComponent<ChainLightningSkill>();
                _skill2 = GetComponent<MeteorFallSkill>();
                CurrentDamage = _player.BuffDamage + PlayerEquipmentManager.Instance.GetCurrentWeaponAttackPower();
                break;
        }

        _skill1?.ResetState();
        _skill2?.ResetState();
    }

    public void TryDamageEnemy(GameObject enemy, Vector3 dir)
    {

        if (_skill1?.IsUsingSkill == true)
        {
            _skill1.TryDamageEnemy(enemy, dir);
            return;
        }
        if (_skill2?.IsUsingSkill == true)
        {
            _skill2.TryDamageEnemy(enemy, dir);
            return;
        }

        _currentWeaponAttack?.TryDamageEnemy(enemy, dir);
    }
}
