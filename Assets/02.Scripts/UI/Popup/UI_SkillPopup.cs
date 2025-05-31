using UnityEngine;

public class UI_SkillPopup : UI_Popup
{
    public void OnClickContinue()
    {
        Debug.Log("Click Continue");
        Close();
        PopUpManager.Instance.PopUpClose();
        GameManager.instance.ChangeState(GameState.Run);
    }
    
}
