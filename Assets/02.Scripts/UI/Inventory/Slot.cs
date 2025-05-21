using System;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public TempItemSO ItemTemp;
    public int ItemCount;
    public Image ItemImage;
    public GameObject SlotHighlight;
    
    public TextMeshProUGUI ItemCountText;
    public GameObject CountText; // 숫자를 끄고 키는 용도
    
    // 아이템 먹었을 때 투명도 조절하기
    private void SetColor(float alpha)
    {
        Color color = ItemImage.color;
        color.a = alpha;
        ItemImage.color = color;
    }
    // 아이템 추가하기
    public void AddItem(TempItemSO item, int count = 1)
    {
        ItemTemp = item;
        ItemImage.sprite = item.ItemImage;
        ItemCount = count;
        if (item.ItemType != TempItemSO.ETempItemType.Equipment)
        {
            CountText.SetActive(true);
            ItemCountText.text = ItemCount.ToString();
        }
        else
        {
            ItemCountText.text = "0";
            CountText.SetActive(false);
        }
        SetColor(1);
    }
    // 아이템이 없는 경우 초기화한다.
    public void ClearSlot()
    {
        ItemTemp = null;
        ItemCount = 0;
        ItemCountText.text = "";
        ItemImage.sprite = null;
        SetColor(0);
    }
    // 마우스 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemTemp != null)
        {
            DragSlot.instance.SlotDrag = this;
            DragSlot.instance.DragSetImage(ItemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }
    // 드래그중이면
    public void OnDrag(PointerEventData eventData)
    {
        if (ItemTemp != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.SlotDrag = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.SlotDrag != null)
        { 
            ChangeSlot();
        }
    }

    public void SetSlotCount(int count)
    {
        ItemCount += count;
        ItemCountText.text = ItemCount.ToString();
        if (ItemCount <= 0)
        {
            ClearSlot();
        }
    }
    public void ChangeSlot()
    {
        TempItemSO temp = ItemTemp;
        int tempCount = ItemCount;
        
        AddItem(DragSlot.instance.SlotDrag.ItemTemp, DragSlot.instance.SlotDrag.ItemCount);
        
        if (temp != null)
        {
            DragSlot.instance.SlotDrag.AddItem(temp,tempCount);
        }
        else
        {
            DragSlot.instance.SlotDrag.ClearSlot();
        }
    }

    public void HighlightSlot(bool isHighlight)
    {
        SlotHighlight.SetActive(isHighlight);
    }
}
