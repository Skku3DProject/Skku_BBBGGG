using UnityEngine;
using UnityEditor;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class UI_EnemyDamageText : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private Enemy _enemy;
    private Canvas _canvas;
    private Camera _mainCamera;
    private RectTransform _rectTransform;

    [SerializeField] private float floatDistance = 50f; // 위로 얼마나 뜨는지 (픽셀 단위)
    [SerializeField] private float duration = 1f;        // 애니메이션 지속 시간
    [SerializeField] private float fadeOutDelay = 0.5f;  // 떠오른 후 사라지기까지 대기 시간

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void UpdateText(float damageValue , Enemy enemy)
    {
        _enemy = enemy;
        _textMeshProUGUI.text = damageValue.ToString();
        Boding();
        Play();
    }

    private void Boding()
    {
        if (_enemy == null) return; 

        // 1) 월드 포인트 → 스크린 포인트
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_mainCamera,
                                             _enemy.UI_offset.position);
        // 2) 스크린 포인트 → 캔버스 로컬좌표
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            screenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _mainCamera,
            out Vector2 localPoint
        );

        // 3) UI 위치 반영
        _rectTransform.anchoredPosition = localPoint;
        gameObject.SetActive(true);
    }

    public void Play()
    {
        // 현재 위치 기준으로 트윈 시작
        Vector2 startPos = _rectTransform.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, floatDistance);

        _textMeshProUGUI.color = new Color(
            _textMeshProUGUI.color.r,
            _textMeshProUGUI.color.g,
            _textMeshProUGUI.color.b,
            1f
        );

        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(_rectTransform.DOAnchorPos(endPos, duration).SetEase(Ease.OutCubic));
        seq.Join(_textMeshProUGUI.DOFade(0f, duration).SetDelay(fadeOutDelay));
        seq.OnComplete(() =>
        {
            UI_Enemy.Instance.CreaDamageText(gameObject); // 풀로 반환
        });
    }
}
