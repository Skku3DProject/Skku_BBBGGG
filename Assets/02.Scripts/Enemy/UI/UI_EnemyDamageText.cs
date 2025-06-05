using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_EnemyDamageText : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private Enemy _enemy;
    private Canvas _canvas;
    private Camera _mainCamera;
    private RectTransform _rectTransform;

    [SerializeField] private float floatDistance = 0f; // ���� �󸶳� �ߴ��� (�ȼ� ����)
    [SerializeField] private float duration = 1f;        // �ִϸ��̼� ���� �ð�
    [SerializeField] private float fadeOutDelay = 0.5f;  // ������ �� ���������� ��� �ð�

    private bool _isPlaying = false;
    private float _floatYOffset = 0f;

    private Color _originColor;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _canvas = GetComponentInParent<Canvas>();
        _originColor = _textMeshProUGUI.color;
    }
    public void UpdateText(Damage damage, Enemy enemy)
    {
        if (damage.From.CompareTag("DeadZone"))
        {
            UI_Enemy.Instance.CreaDamageText(gameObject);
        }
        _enemy = enemy;
        _textMeshProUGUI.text = damage.Value.ToString();

        

        if (damage.Value < 500)
        {
            _textMeshProUGUI.color = Color.white;
        }
        else
        {
            _textMeshProUGUI.color = _originColor;

        }

        gameObject.SetActive(true);
        Play();
    }

    private void LateUpdate()
    {
        Boding();
    }

    private void Boding()
    {
        if (!_isPlaying || _enemy == null) return;

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_mainCamera, _enemy.transform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            screenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCamera,
            out Vector2 localPoint
        );

        // ���� �ߴ� ������ �߰�
        localPoint.y += _floatYOffset;
        _rectTransform.anchoredPosition = localPoint;
    }

    public void Play()
    {
        _isPlaying = true;
        _floatYOffset = 0f;

        _textMeshProUGUI.color = new Color(
            _textMeshProUGUI.color.r,
            _textMeshProUGUI.color.g,
            _textMeshProUGUI.color.b,
            1f
        );

        // �������� + ���� ���̵�
        DG.Tweening.Sequence seq = DOTween.Sequence();
        _rectTransform.localScale = Vector3.one;
        seq.Join(_rectTransform.DOScale(3f, duration).SetEase(Ease.OutCubic));
        seq.Join(DOTween.To(() => _floatYOffset, x => _floatYOffset = x, floatDistance, duration).SetEase(Ease.OutCubic));
        seq.Join(_textMeshProUGUI.DOFade(0f, duration).SetDelay(fadeOutDelay));
        //.Append(_rectTransform.DOScale(1f, duration * 0.5f).SetEase(Ease.InBack));
        seq.OnComplete(() =>
        {
            _isPlaying = false;

            UI_Enemy.Instance.CreaDamageText(gameObject);
        });
    }
}
