using UnityEngine;

public class UIInputHandler : MonoBehaviour
{
    private bool _canTab = false;
    private void Update()
    {
        if(GameManager.instance.CurrentState == GameState.Pause)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.B) && PlayerModeManager.Instance.CurrentMode != EPlayerMode.Build)
        {
            PopUpManager.Instance.Open(EPopupType.UI_BuildMenu, GameManager.instance.ContinueGame);
            UI_TowerBuildMenu.isBuildMode = true;
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            PopUpManager.Instance.Open(EPopupType.UI_SkillPopup, GameManager.instance.ContinueGame);
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !_canTab)
        {
            
            UIManager.instance.UI_AppearCurrency();
            _canTab = !_canTab;
        }
        else if(Input.GetKeyDown(KeyCode.Tab) && _canTab)
        {
            UIManager.instance.UI_DisappearCurrency();
            _canTab = !_canTab;
        }
    }
}
