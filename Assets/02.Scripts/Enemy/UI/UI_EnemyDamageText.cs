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

	[SerializeField] private float floatDistance = 0f; // ���� �󸶳� �ߴ��� (�ȼ� ����)
	[SerializeField] private float duration = 1f;        // �ִϸ��̼� ���� �ð�
	[SerializeField] private float fadeOutDelay = 0.5f;  // ������ �� ���������� ��� �ð�

	private bool _isPlaying = false;
	private float _floatYOffset = 0f;

	private void Awake()
	{
		_mainCamera = Camera.main;
		_rectTransform = GetComponent<RectTransform>();
		_textMeshProUGUI = GetComponent<TextMeshProUGUI>();
		_canvas = GetComponentInParent<Canvas>();
	}

	public void UpdateText(float damageValue, Enemy enemy)
	{
		_enemy = enemy;
		_textMeshProUGUI.text = damageValue.ToString();
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
		seq.Join(DOTween.To(() => _floatYOffset, x => _floatYOffset = x, floatDistance, duration).SetEase(Ease.OutCubic));
		seq.Join(_textMeshProUGUI.DOFade(0f, duration).SetDelay(fadeOutDelay));
		seq.OnComplete(() =>
		{
			_isPlaying = false;
			UI_Enemy.Instance.CreaDamageText(gameObject);
		});
	}
}
