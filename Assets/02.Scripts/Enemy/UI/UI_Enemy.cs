using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UI_Enemy : MonoBehaviour
{
    public static UI_Enemy Instance = null;

    public GameObject HPbar;

    public float maxDistance = 50f; // 최대 표시 거리
    public int maxVisibleHealthBars = 30; // 최대 표시 개수

    private List<UI_EnemyHpbar> _hpBars;
    private Camera _mainCamera;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateAllHealthBars();
    }

    public void SetHPBarMaxSize(int capacity)
    {
        if (_hpBars == null)
        {
            _hpBars = new List<UI_EnemyHpbar>(capacity);
        }

    }

    public void SetHpBarToEnemy(Enemy enemy)
    {
        GameObject hpBar = Instantiate(HPbar, transform);
        UI_EnemyHpbar hpBarComponent = hpBar.GetComponent<UI_EnemyHpbar>();

        hpBar.SetActive(false);
        hpBarComponent.SetHpBarToEnemy(enemy);
        _hpBars.Add(hpBarComponent);
    }
    public void RemoveHpBar(Enemy enemy)
    {
        for (int i = _hpBars.Count - 1; i >= 0; i--)
        {
            if (_hpBars[i].GetEnemy() == enemy)
            {
                if (_hpBars[i] != null)
                    _hpBars[i].gameObject.SetActive(false);
                _hpBars.RemoveAt(i);
                break;
            }
        }
    }

    private void UpdateAllHealthBars()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }

        // 유효하지 않은 체력바 제거
        for (int i = _hpBars.Count - 1; i >= 0; i--)
        {
            if (_hpBars[i] == null || _hpBars[i].GetEnemy() == null)
            {
                if (_hpBars[i] != null)
                    _hpBars[i].gameObject.SetActive(false);
                _hpBars.RemoveAt(i);
            }
        }

        // 거리별 정렬을 위한 리스트
        List<HealthBarDistanceInfo> healthBarInfos = new List<HealthBarDistanceInfo>();

        Vector3 cameraPos = _mainCamera.transform.position;

        // 각 체력바의 거리와 가시성 계산
        foreach (var hpBar in _hpBars)
        {
            Enemy enemy = hpBar.GetEnemy();
            if (enemy == null) continue;

            Vector3 enemyPos = enemy.transform.position;
            float distance = Vector3.Distance(cameraPos, enemyPos);

            // 기본 가시성 체크
            bool isVisible = CheckVisibility(enemyPos, distance);

            healthBarInfos.Add(new HealthBarDistanceInfo
            {
                hpBar = hpBar,
                distance = distance,
                isVisible = isVisible
            });
        }

        // 거리 순으로 정렬 (가까운 것부터)
        healthBarInfos.Sort((a, b) => a.distance.CompareTo(b.distance));

        // 최대 표시 개수 제한 적용
        int visibleCount = 0;
        foreach (var info in healthBarInfos)
        {
            bool shouldShow = info.isVisible && visibleCount < maxVisibleHealthBars;

            info.hpBar.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                visibleCount++;
            }
        }
    }

    private bool CheckVisibility(Vector3 worldPos, float distance)
    {
        // 1. 거리 체크
        if (distance > maxDistance) return false;

        // 2. 카메라 뒤쪽 및 화면 밖 체크
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

        // 카메라 뒤쪽
        if (screenPos.z < 0) return false;

        // 화면 밖 (여유분 포함)
        float margin = 100f;
        if (screenPos.x < -margin || screenPos.x > Screen.width + margin ||
            screenPos.y < -margin || screenPos.y > Screen.height + margin)
            return false;

        return true;
    }

    // 모든 체력바 강제 업데이트
    public void ForceUpdateAll()
    {
        UpdateAllHealthBars();
    }

    // 특정 체력바만 업데이트
    public void UpdateSpecificHealthBar(Enemy enemy)
    {
        foreach (var hpBar in _hpBars)
        {
            if (hpBar.GetEnemy() == enemy)
            {
                Vector3 enemyPos = enemy.transform.position;
                float distance = Vector3.Distance(_mainCamera.transform.position, enemyPos);
                bool isVisible = CheckVisibility(enemyPos, distance);
                hpBar.gameObject.SetActive(isVisible);
                break;
            }
        }
    }

    // 전체 체력바 시스템 비활성화
    public void DeActivateAllHpBars()
    {
        foreach (var hpBar in _hpBars)
        {
            if (hpBar != null)
                hpBar.gameObject.SetActive(false);
        }
    }

    // 전체 체력바 시스템 활성화
    public void ActivateAllHpBars()
    {
        ForceUpdateAll();
    }

    // 디버그 정보
    public void GetDebugInfo(out int totalCount, out int activeCount)
    {
        totalCount = _hpBars.Count;
        activeCount = 0;

        foreach (var hpBar in _hpBars)
        {
            if (hpBar != null && hpBar.gameObject.activeSelf)
                activeCount++;
        }
    }
}

