using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 팝업으로 사용할 UI들 정리하기
public enum EPopupType
{
    UI_OptionPopup,
    UI_BuildMenu,
    UI_SkillPopup,
    UI_RewardPopup
}
public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;
    public List<UI_Popup> Popups;
    private Stack<UI_Popup>  _openPopups = new Stack<UI_Popup>();
    public Image PauseBackground;
    
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
                while (_openPopups.Count > 0)
                {
                    UI_Popup popup = _openPopups.Pop();
                    bool opened = popup.isActiveAndEnabled;
                    popup.Close();
                    GameManager.instance.ChangeState(GameState.Run);
                    // Peek() 대신 그냥 break
                    if (opened)
                        break;
                }
                //while (true)
                //{
                //    UI_Popup popup = _openPopups.Pop();
                //    bool opened = popup.isActiveAndEnabled;
                //    popup.Close();

                //    if (opened || _openPopups.Peek() == null)
                //    {
                //        break;
                //    }
                //} 
            }
            else
            { 
                Open(EPopupType.UI_OptionPopup, GameManager.instance.ContinueGame);
            }
        }
    }
    public T OpenPopup<T>(EPopupType popupType, Action closeCallback = null) where T : UI_Popup
    {
        foreach (UI_Popup pop in Popups)
        {
            if (pop.name == popupType.ToString())
            {
                pop.Open(closeCallback);
                _openPopups.Push(pop);
                return pop as T;
            }
        }
        return null;
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

    public void PopUpClose(Action callBack = null)
    {
        _openPopups.Pop();
    }
}
