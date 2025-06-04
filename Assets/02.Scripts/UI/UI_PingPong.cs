using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_PingPong : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        
        _text.DOFade(1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
    }

    private void OnDisable()
    {
        _text.DOKill();
    }
}
