using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public TempItem TempItem;
    public Image ItemImage;

    // 아이템 먹었을 때 투명도 조절하기
    private void SetColor(float alpha)
    {
        Color color = ItemImage.color;
        color.a = alpha;
        ItemImage.color = color;
    }
    // 아이템 추가하기
    public void AddItem(TempItem item)
    {
        TempItem = item;
        ItemImage.sprite = item.ItemImage;
        
        SetColor(1);
    }
    // 아이템이 없는 경우 초기화한다.
    public void ClearSlot()
    {
        TempItem = null;
        ItemImage.sprite = null;
        SetColor(0);
    }
    
}
