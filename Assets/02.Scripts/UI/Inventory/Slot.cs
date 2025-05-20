using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public TempItem ItemTemp;
    public Image ItemImage;

    private void Start()
    {
        ItemTemp= GetComponentInChildren<TempItem>();
        AddItem(ItemTemp);
    }

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
        ItemTemp = item;
        ItemImage.sprite = item.ItemImage;
        
        SetColor(1);
    }
    // 아이템이 없는 경우 초기화한다.
    public void ClearSlot()
    {
        ItemTemp = null;
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
            DragSlot.instance.SetDraggedPosition(eventData);
        }
    }
    // 드래그중이면
    public void OnDrag(PointerEventData eventData)
    {
        if (ItemTemp != null)
        {
            DragSlot.instance.SetDraggedPosition(eventData);
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

    public void ChangeSlot()
    {
        TempItem temp = ItemTemp;
        
        AddItem(DragSlot.instance.SlotDrag.ItemTemp);
        if (temp != null)
        {
            DragSlot.instance.SlotDrag.AddItem(temp);
        }
        else
        {
            DragSlot.instance.SlotDrag.ClearSlot();
        }
    }
}
