using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_OptionPopup : UI_Popup
{
    public void GoMainMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void OnClickContinue()
    {
        gameObject.SetActive(false);
        Close();
        PopUpManager.Instance.PopUpClose();
        GameManager.instance.ChangeState(GameState.Run);
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
