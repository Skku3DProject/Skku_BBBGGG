using UnityEngine;

[CreateAssetMenu(fileName = "TempItemSO", menuName = "Scriptable Objects/TempItemSO")]
public class TempItemSO : ScriptableObject
{
    public enum ETempItemType
    {
        Equipment,
        Used,
        Ingredient,
        ETC,
    }
    public string ItemName;
    public ETempItemType ItemType;// 아이템 종류
    public EquipmentType EquipmentType;
    public Sprite ItemImage;      // 인벤토르에 올라갈 이미지
    public GameObject ItemPrefab; // 아이템 프리펩
}
