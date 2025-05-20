using UnityEngine;

public class UI_OptionPopup : UI_Popup
{
    public void OnClickRestart()
    {
        GameManager.instance.RestartGame();
    }

    public void OnClickContinue()
    {
        GameManager.instance.ChangeState(GameManager.GameState.Run);
    }

    public void OnClickQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
}
