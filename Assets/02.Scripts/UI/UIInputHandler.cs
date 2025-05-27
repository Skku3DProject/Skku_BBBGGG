using UnityEngine;

public class UIInputHandler : MonoBehaviour
{
    private void Update()
    {
        if(GameManager.instance.CurrentState == GameState.Pause)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.B) && PlayerModeManager.Instance.CurrentMode != EPlayerMode.Build)
        {
            PopUpManager.Instance.Open(EPopupType.UI_BuildMenu, GameManager.instance.ContinueGame);
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            PopUpManager.Instance.Open(EPopupType.UI_SkillPopup, GameManager.instance.ContinueGame);
        }
    }
}
