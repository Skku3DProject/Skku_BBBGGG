using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyHpbar : MonoBehaviour
{
    public Slider Slider;
    private Camera _mainCamera;
    private Enemy _enemy;
    private RectTransform _rt;
    private Canvas _canvas;
   
    private void Awake()
    {
        _mainCamera = Camera.main;
        _rt = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    private void LateUpdate()
    {
        if (gameObject.activeSelf && _enemy != null)
        {
            BillBoarding();
        }
    }

    public void BillBoarding()
    {
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
        _rt.anchoredPosition = localPoint;
    }
    public void SetHpBarToEnemy(Enemy enemy)
    {
        _enemy = enemy;
    }

    public Enemy GetEnemy()
    {
        return _enemy;
    }

    public void UpdateHealth(float health)
    {
        Slider.value = health;
    }

    public void Initialized()
    {
        Slider.value = 1;
	}
  
}
