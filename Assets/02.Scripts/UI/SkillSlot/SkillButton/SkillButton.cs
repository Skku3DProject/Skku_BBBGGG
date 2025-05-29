using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Button ChildButton;
    private bool _open = false;
   
    public void OnClickAppearLevelUp()
    {
        _open = !_open;
        ChildButton.gameObject.SetActive(_open);
    }
}
