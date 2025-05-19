using UnityEditor.UIElements;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // 슬롯들 받아오기
    [SerializeField] private GameObject _slotParent;
    // 슬롯들 묶음
    [SerializeField] private Slot[] _slots;

    // 슬롯 자식들 가져오기
    private void Start()
    {
        _slots = _slotParent.GetComponentsInChildren<Slot>();
    }
    
    public void AcquireItem(TempItem item)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].TempItem == null)
            {
                _slots[i].AddItem(item);
                return;
            }
        }
    }
}
