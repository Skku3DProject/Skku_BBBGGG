using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class UI_OptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource ClickSound;
    public AudioClip ClickSoundClip;
    private RectTransform _rectTransform;
    private void Awake()
    {
        ClickSound = GetComponent<AudioSource>();
        _rectTransform = GetComponent<RectTransform>();
        
    }

    private void Start()
    {
        DOTween.Init();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _rectTransform.DOScale(1.1f, 0.2f).SetEase(Ease.OutCirc)
            .SetUpdate(true);
        ClickSound.PlayOneShot(ClickSoundClip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.DOScale(1, 0.2f).SetEase(Ease.OutCirc)
            .SetUpdate(true);
    }

    private void OnDisable()
    {
        _rectTransform.DOScale(1, 0.2f);
    }
}
