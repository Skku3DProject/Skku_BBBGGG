using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum EquipmentType
{
    Sword,
    Bow,
    Magic,
    Pickaxe
}
public class PlayerEquipmentController : MonoBehaviour
{
    public static PlayerEquipmentController Instance;


    [Header("장비 오브젝트")]
    public GameObject Sword;
    public GameObject Shield;
    public GameObject Pickaxe;
    public GameObject Bow;
    public GameObject Magic;

    private Animator _playerAnimation;

    // 현재 장비 타입
    private EquipmentType _currentEquipType = EquipmentType.Sword;

    // 장비 타입 배열 (순환용)
    private EquipmentType[] weapons = { EquipmentType.Sword, EquipmentType.Bow, EquipmentType.Magic };
    private EquipmentType tool = EquipmentType.Pickaxe;

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
        if (Input.GetKeyDown(KeyCode.Q)) // 무기 순환
        {
            CycleWeapons();
        }
        if (Input.GetKeyDown(KeyCode.E)) // 도구 활성화
        {
            SetEquipment(tool);
        }
    }

    private void CycleWeapons()
    {
        int currentIndex = System.Array.IndexOf(weapons, _currentEquipType);
        int nextIndex = (currentIndex + 1) % weapons.Length;
        SetEquipment(weapons[nextIndex]);
    }

    private void SetEquipment(EquipmentType equipType)
    {
        _currentEquipType = equipType;

        // 모두 비활성화
        Sword.SetActive(false);
        Shield.SetActive(false);
        Pickaxe.SetActive(false);
        Bow.SetActive(false);
        Magic.SetActive(false);

        // 해당 장비 활성화
        switch (equipType)
        {
            case EquipmentType.Sword:
                Sword.SetActive(true);
                Shield.SetActive(true);
                break;
            case EquipmentType.Pickaxe:
                Pickaxe.SetActive(true);
                break;
            case EquipmentType.Bow:
                Bow.SetActive(true);
                break;
            case EquipmentType.Magic:
                Magic.SetActive(true);
                break;
        }

        // 애니메이터 레이어 활성화
        ActivateAnimationLayer(equipType.ToString());
    }

    private void ActivateAnimationLayer(string layerName)
    {
        for (int i = 0; i < _playerAnimation.layerCount; i++)
        {
            string currentLayerName = _playerAnimation.GetLayerName(i);
            _playerAnimation.SetLayerWeight(i, currentLayerName == layerName ? 1f : 0f);
        }
    }

    public EquipmentType GetCurrentEquipType()
    {
        Debug.Log("현재 무기 타입은" + _currentEquipType + "입니다.");
        return _currentEquipType;
        
    }


    //현재 장비에 맞는 공격력 변환
    public float GetCurrentWeaponAttackPower()
    {
        /*foreach (var weaponSO in WeaponAttackSO)
        {
            Debug.Log($"무기 이름 확인: {weaponSO.WeaponName}, 공격력: {weaponSO.AttackPower}");
            if (weaponSO.WeaponName==_currentEquipType.ToString())
            {
                Debug.Log($"매칭된 무기 공격력 반환: {weaponSO.AttackPower}");
                return weaponSO.AttackPower;
            }
        }

        return 0f;*/

        Debug.Log($"공격력 요청: 현재 장착 장비 타입은 {_currentEquipType}");

        if (WeaponAttackSO == null || WeaponAttackSO.Count == 0)
        {
            Debug.LogWarning("WeaponAttackSO 리스트가 비어 있음!");
            return 0f;
        }

        foreach (var weaponSO in WeaponAttackSO)
        {
            Debug.Log($"WeaponSO: 이름={weaponSO.WeaponName}, 공격력={weaponSO.AttackPower}");

            if (weaponSO.WeaponName.Trim().ToLower() == _currentEquipType.ToString().ToLower())
            {
                Debug.Log($"매칭 성공: {weaponSO.WeaponName}의 공격력은 {weaponSO.AttackPower}");
                return weaponSO.AttackPower;
            }
        }

        Debug.LogWarning("공격력 매칭 실패: 해당 무기 이름이 없음");
        return 0f;
    }//-> PlayerAttact에서 사용중
}
