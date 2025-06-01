using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject SelectVFX;
    private Button _button;
    private bool _canSelected => _button.interactable;
    
    private void OnEnable()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_canSelected)
        {
            return;
        }
        SelectVFX.SetActive(true);   
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_canSelected)
        {
            return;
        }
        SelectVFX.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_canSelected)
        {
            return;
        }
        SelectVFX.SetActive(false);
    }
    public void OnDisable()
    {
        
        SelectVFX.SetActive(false);
    }
}
