using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    public static DragSlot instance;
    
    public Slot SlotDrag;

    public Image _imageItem;
    private void Start()
    {
        instance = this; 
    }

    public void DragSetImage(Image image)
    {
        _imageItem.sprite = image.sprite;
        SetColor(0.7f);
    }

    public void SetColor(float alpha)
    {
        Color color = _imageItem.color;
        color.a = alpha;
        _imageItem.color = color;
    }
}
