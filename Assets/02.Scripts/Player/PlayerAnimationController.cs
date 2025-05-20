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
    [Header("장비 오브젝트")]
    public GameObject Sword;
    public GameObject Shield;
    public GameObject Pickaxe;
    public GameObject Bow;
    public GameObject Magic;

    [Header("애니메이터")]
    public Animator animator;

    // 현재 장비 타입
    private EquipmentType _currentEquipType = EquipmentType.Sword;

    // 장비 타입 배열 (순환용)
    private EquipmentType[] weapons = { EquipmentType.Sword, EquipmentType.Bow, EquipmentType.Magic };
    private EquipmentType tool = EquipmentType.Pickaxe;

    void Start()
    {
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
        for (int i = 0; i < animator.layerCount; i++)
        {
            string currentLayerName = animator.GetLayerName(i);
            animator.SetLayerWeight(i, currentLayerName == layerName ? 1f : 0f);
        }
    }
}
