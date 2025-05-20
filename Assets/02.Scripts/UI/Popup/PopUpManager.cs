using System;
using System.Collections.Generic;
using UnityEngine;

// 팝업으로 사용할 UI들 정리하기
public enum EPopupType
{
    UI_OptionPopup,
}
public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;
    public List<UI_Popup> Popups;
    private Stack<UI_Popup>  _openPopups = new Stack<UI_Popup>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_openPopups.Count > 0)
            {
                while (true)
                {
                    UI_Popup popup = _openPopups.Pop();
                    bool opened = popup.isActiveAndEnabled;
                    popup.Close();

                    if (opened || _openPopups.Peek() == null)
                    {
                        break;
                    }
                } 
            }
            else
            {
                GameManager.instance.ChangeState(GameState.Pause);
            }
        }
    }

    public void Open(EPopupType popupType, Action callBack = null)
    {
        PopUpOpen(popupType.ToString(), callBack);
    }

    public void PopUpOpen(string popupName, Action closeCallback)
    {
        foreach (UI_Popup pop in Popups)
        {
            if (pop.name == popupName)
            {
                pop.Open(closeCallback);
                _openPopups.Push(pop);
                break;
            }
        }
    }
}
