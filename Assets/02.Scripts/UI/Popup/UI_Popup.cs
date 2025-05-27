using System;
using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    public Action _closeCallback;
    
    public void Open(Action callback = null)
    {
        _closeCallback = callback;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameObject.SetActive(true);
        GameManager.instance.ChangeState(GameState.Pause);
        PopUpManager.Instance.PauseBackground.gameObject.SetActive(true);
    }

    public void Close()
    {
        _closeCallback?.Invoke();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        gameObject.SetActive(false);

    }
}
