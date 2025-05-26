using UnityEngine;

public class UIInputHandler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && PlayerModeManager.Instance.CurrentMode != EPlayerMode.Build)
        {
            PopUpManager.Instance.Open(EPopupType.UI_BuildMenu);
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            PopUpManager.Instance.Open(EPopupType.UI_SkillPopup);
        }
    }
}
