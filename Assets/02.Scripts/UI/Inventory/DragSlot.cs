using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    public static DragSlot instance;
    
    public Slot SlotDrag;

    [SerializeField] private Image _imageItem;
    private RectTransform _rectTransform;
    private Canvas _canvas;

    private void Start()
    {
        instance = this;   
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void DragSetImage(Image image)
    {
        _imageItem = image;
        SetColor(0.7f);
    }

    public void SetColor(float alpha)
    {
        Color color = _imageItem.color;
        color.a = alpha;
        _imageItem.color = color;
    }

    public void SetDraggedPosition(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera, // null이면 Overlay 모드
                out localPoint))
        {
            _rectTransform.localPosition = localPoint;
        }
    }
}
