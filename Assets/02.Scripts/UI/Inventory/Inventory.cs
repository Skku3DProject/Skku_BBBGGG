using UnityEngine;

public class Inventory : MonoBehaviour
{
    public TempItemSO[] Items;
    private int _keyOffSet = -1;
    public bool InventoryOpen = false;

    public GameObject[] SkillInventory;
    private TempItemSO _selectedItem;
    
    // 슬롯들 받아오기
    [SerializeField] private GameObject _inventoryBase;

    [SerializeField] private GameObject _gridSystem;
    // 슬롯들 묶음
    [SerializeField] private Slot[] _slots;
    
    // 슬롯 자식들 가져오기
    private void Start()
    {
        _slots = _inventoryBase.GetComponentsInChildren<Slot>();
        TestAcquire();
    }

    private void Update()
    {
        OpenInventory();
        SelectSlot();
    }

    private void SelectSlot()
    {
        for (int i = 0; i < Mathf.Min(9, _slots.Length); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (_keyOffSet != -1)
                {
                    _slots[_keyOffSet].HighlightSlot(false);
                }

                _keyOffSet = i;
                _slots[_keyOffSet].HighlightSlot(true);
                _selectedItem = _slots[_keyOffSet].ItemTemp;      
               
                SkillManager.instance.SwitchSkilltory(_selectedItem.EquipmentType);
            } 
            
        }
        
    }
    // 인벤토리 온 오프
    private void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryOpen = !InventoryOpen;
            if (InventoryOpen)
            {
                _inventoryBase.SetActive(true);
            }
            else
            {
                _inventoryBase.SetActive(false);
            }
        }
    }
    // 아이템 먹기 = 빈칸에 들어가기
    public void AcquireItem(TempItemSO item, int amount = 1)
    {
        if (item.ItemType != TempItemSO.ETempItemType.Equipment)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].ItemTemp != null)
                {
                    if (_slots[i].ItemTemp.ItemName == item.ItemName)
                    {
                        _slots[i].SetSlotCount(amount);
                        return;
                    }
                }
            }
        }
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].ItemTemp == null)
            {
                _slots[i].AddItem(item, amount);
                return;
            }

        }   
    }

    public void TestAcquire()
    {
        foreach (var item in Items)
        {
            AcquireItem(item);
        }
    }

    public TempItemSO GetItem()
    {
        return _selectedItem;
    }
}
