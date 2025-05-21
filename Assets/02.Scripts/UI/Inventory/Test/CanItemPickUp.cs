using TMPro;
using UnityEngine;

public class CanItemPickUp : MonoBehaviour
{
    public float pickUpDistance = 5f; // 아이템을 줍기 위한 거리
    private bool _canPickUp = false; // 아이템을 줍을 수 있는지 여부
    private RaycastHit _hitInfo;
    public LayerMask _layerMask;
    public TextMeshProUGUI _text;
    private Inventory _inventory;
    private void Update()
    {
        CheckItem();
        TryAction();
    }
    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            CheckItem();
            ItemPickedUp();
        }
    }
    // 아이템을 주울 수 있는 가? = 아이템이 있는지
    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.forward, out _hitInfo, pickUpDistance, _layerMask))
        {
            AppearItemInfo();       
        }
        else
        {
            DisappearItemInfo();
        }
    }
    // 주울 수 있는 아이템 이름 띄우기
    private void AppearItemInfo()
    {
        _canPickUp = true;
        _text.gameObject.SetActive(true);
        _text.text = $"{_hitInfo.transform.GetComponent<PickUpItem>().ItemSO.ItemName}";
    }
    // 이름 없애기
    private void DisappearItemInfo()
    {
        _canPickUp = false;
        _text.gameObject.SetActive(false);
    }

    private void ItemPickedUp()
    {
        if (!_canPickUp)
        {
            return;
        }

        if (_hitInfo.transform != null)
        {
            Debug.Log($"{_hitInfo.transform.GetComponent<PickUpItem>().ItemSO.ItemName} 획득");
            _inventory.AcquireItem(_hitInfo.transform.GetComponent<PickUpItem>().ItemSO);
            Destroy(_hitInfo.transform.gameObject);
            DisappearItemInfo();
        }
        
    }
}
