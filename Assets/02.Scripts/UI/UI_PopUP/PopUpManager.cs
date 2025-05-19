using System.Collections.Generic;
using UnityEngine;

public enum EPopupType
{
    UI_OptionPopup,
    
}
public class PopUpManager : MonoBehaviour
{
    [Header("팝업 UI 참조")]
    public List<UI_PopUp> PopUps;
    private Stack<UI_PopUp> _popUpStacks = new Stack<UI_PopUp>();

    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Escape))
            
    }


    public void Open()
    {
        
    }

    public void PopOpen()
    {
        
    }
}
