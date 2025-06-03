using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject SelectVFX;
    public AudioClip EnterSound;
    
    private Button _button;
    private AudioSource _audio;
    private bool _canSelected => _button.interactable;
    
    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _audio = GetComponent<AudioSource>();
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
        _audio.PlayOneShot(EnterSound);
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
