using System;
using UnityEngine;

public class UI_PopUp : MonoBehaviour
{
    private Action _closseCallback;

    public void Open(Action close = null)
    {
        _closseCallback = close;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        _closseCallback?.Invoke();
        gameObject.SetActive(false);
    }
}
