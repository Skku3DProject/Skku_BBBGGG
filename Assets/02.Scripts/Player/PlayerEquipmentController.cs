using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum EquipmentType
{
    Sword,
    Bow,
    Magic,
    Pickaxe,
    Block
}
public class PlayerEquipmentController : MonoBehaviour
{
    public static PlayerEquipmentController Instance;

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
    private EquipmentType _currentEquipType = EquipmentType.Sword;

    //무기 이름과 공격력
    public List<WeaponAttackSO> WeaponAttackSO;


    void Start()
    {
        Instance = this;

        _playerAnimation = GetComponent<Animator>();
        SetEquipment(_currentEquipType);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) // 검
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Weapon);
            SetEquipment(EquipmentType.Sword);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))//활
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Weapon);
            SetEquipment(EquipmentType.Bow);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))//지팡이
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Weapon);
            SetEquipment(EquipmentType.Magic);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))//곡괭이
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Pickaxe);
            SetEquipment(EquipmentType.Pickaxe);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))//블럭
        {
            PlayerModeManager.Instance.SetMode(EPlayerMode.Block);
            SetEquipment(EquipmentType.Block); //블럭
        }
    }


    private void SetEquipment(EquipmentType equipType)
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
            case EquipmentType.Sword:
                Sword.SetActive(true);
                Shield.SetActive(true);
                _playerAnimation.runtimeAnimatorController = SwordOverrideController;
                break;
            case EquipmentType.Pickaxe:
                Pickaxe.SetActive(true);
                _playerAnimation.runtimeAnimatorController = PickaxeOverrideController;
                break;
            case EquipmentType.Bow:
                Bow.SetActive(true);
                Arrow.SetActive(true);
                _playerAnimation.runtimeAnimatorController = BowOverrideController;
                break;
            case EquipmentType.Magic:
                Magic.SetActive(true);
                _playerAnimation.runtimeAnimatorController = MagicOverrideController;
                break;
            case EquipmentType.Block:
                Block.SetActive(true);
                _playerAnimation.runtimeAnimatorController = BlockOverrideController;
                break;
        }
    }

    public EquipmentType GetCurrentEquipType()
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
