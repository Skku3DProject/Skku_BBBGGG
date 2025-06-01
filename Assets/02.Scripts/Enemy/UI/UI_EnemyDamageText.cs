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

    [SerializeField] private float floatDistance = 50f; // ���� �󸶳� �ߴ��� (�ȼ� ����)
    [SerializeField] private float duration = 1f;        // �ִϸ��̼� ���� �ð�
    [SerializeField] private float fadeOutDelay = 0.5f;  // ������ �� ���������� ��� �ð�

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

        // 1) ���� ����Ʈ �� ��ũ�� ����Ʈ
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_mainCamera,
                                             _enemy.UI_offset.position);
        // 2) ��ũ�� ����Ʈ �� ĵ���� ������ǥ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            screenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _mainCamera,
            out Vector2 localPoint
        );

        // 3) UI ��ġ �ݿ�
        _rectTransform.anchoredPosition = localPoint;
        gameObject.SetActive(true);
    }

    public void Play()
    {
        // ���� ��ġ �������� Ʈ�� ����
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
            UI_Enemy.Instance.CreaDamageText(gameObject); // Ǯ�� ��ȯ
        });
    }
}
