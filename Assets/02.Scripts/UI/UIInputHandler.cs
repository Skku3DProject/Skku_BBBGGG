using UnityEngine;

public class UIInputHandler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            PopUpManager.Instance.Open(EPopupType.UI_BuildMenu);
        }
    }
}
