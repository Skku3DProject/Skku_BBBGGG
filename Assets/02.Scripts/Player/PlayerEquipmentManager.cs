using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum EEquipmentType
{
    Sword,
    Bow,
    Magic,
    Pickaxe,
    Block
}
public class PlayerEquipmentManager : MonoBehaviour
{
    public static PlayerEquipmentManager Instance;

    [Header("무기별 오버라이드 컨트롤러")]
    public RuntimeAnimatorController SwordOverrideController;
    public RuntimeAnimatorController BowOverrideController;
    public RuntimeAnimatorController MagicOverrideController;
    public RuntimeAnimatorController PickaxeOverrideController;
    public RuntimeAnimatorController BlockOverrideController;

    [Header("장비 오브젝트")]
    public GameObject Sword;
    public GameObject Shield;
    public GameObject Bow;
    public GameObject Arrow;
    public GameObject Magic;
    public GameObject Pickaxe;
    public GameObject Block;

    private Animator _playerAnimation;

    // 현재 장비 타입
    private EEquipmentType _currentEquipType = EEquipmentType.Sword;

    //무기 이름과 공격력
    public List<WeaponAttackSO> WeaponAttackSO;

    public event Action<EEquipmentType> OnChangedEquipment;
    private void Awake()
    {
        Instance = this;

    }

    void Start()
    {

        _playerAnimation = GetComponent<Animator>();
        SetEquipment(_currentEquipType);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) // 검
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Weapon);
            SetEquipment(EEquipmentType.Sword);
            OnChangedEquipment?.Invoke(_currentEquipType);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))//활
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Weapon);
            SetEquipment(EEquipmentType.Bow);
            OnChangedEquipment?.Invoke(_currentEquipType);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))//지팡이
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Weapon);
            SetEquipment(EEquipmentType.Magic);
            OnChangedEquipment?.Invoke(_currentEquipType);

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))//곡괭이
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Pickaxe);
            SetEquipment(EEquipmentType.Pickaxe);
            OnChangedEquipment?.Invoke(_currentEquipType);

        }
        if (Input.GetKeyDown(KeyCode.Alpha5))//블럭
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Block);
            SetEquipment(EEquipmentType.Block); //블럭
            OnChangedEquipment?.Invoke(_currentEquipType);

        }

    }


    private void SetEquipment(EEquipmentType equipType)
    {
        _currentEquipType = equipType;

        // 모두 비활성화
        Sword.SetActive(false);
        Shield.SetActive(false);
        Pickaxe.SetActive(false);
        Bow.SetActive(false);
        Arrow.SetActive(false);
        Magic.SetActive(false);
        Block.SetActive(false);

        // 해당 장비 활성화
        switch (equipType)
        {
            case EEquipmentType.Sword:
                Sword.SetActive(true);
                Shield.SetActive(true);
                _playerAnimation.runtimeAnimatorController = SwordOverrideController;
                break;
            case EEquipmentType.Pickaxe:
                Pickaxe.SetActive(true);
                _playerAnimation.runtimeAnimatorController = PickaxeOverrideController;
                break;
            case EEquipmentType.Bow:
                Bow.SetActive(true);
                Arrow.SetActive(true);
                _playerAnimation.runtimeAnimatorController = BowOverrideController;
                break;
            case EEquipmentType.Magic:
                Magic.SetActive(true);
                _playerAnimation.runtimeAnimatorController = MagicOverrideController;
                break;
            case EEquipmentType.Block:
                Block.SetActive(true);
                _playerAnimation.runtimeAnimatorController = BlockOverrideController;
                break;
        }
    }

    public EEquipmentType GetCurrentEquipType()
    {
        return _currentEquipType; 
    }


    //현재 장비에 맞는 공격력 변환
    public float GetCurrentWeaponAttackPower()
    {

        if (WeaponAttackSO == null || WeaponAttackSO.Count == 0)
        {
           // Debug.LogWarning("WeaponAttackSO 리스트가 비어 있음!");
            return 0f;
        }

        foreach (var weaponSO in WeaponAttackSO)
        {

            if (weaponSO.WeaponName.Trim().ToLower() == _currentEquipType.ToString().ToLower())
            {
                return weaponSO.AttackPower;
            }
        }

        return 0f;
    }
}
