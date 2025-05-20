using System;
using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    public Action _closeCallback;
    
    public void Open(Action callback = null)
    {
        _closeCallback = callback;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        _closeCallback?.Invoke();
        gameObject.SetActive(false);
    }
}
