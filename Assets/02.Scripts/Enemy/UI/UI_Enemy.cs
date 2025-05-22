using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UI_Enemy : MonoBehaviour
{
    public static UI_Enemy Instance = null;

    public GameObject HPbar;

    public float maxDistance = 50f; // �ִ� ǥ�� �Ÿ�
    public int maxVisibleHealthBars = 30; // �ִ� ǥ�� ����

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

        // ��ȿ���� ���� ü�¹� ����
        for (int i = _hpBars.Count - 1; i >= 0; i--)
        {
            if (_hpBars[i] == null || _hpBars[i].GetEnemy() == null)
            {
                if (_hpBars[i] != null)
                    _hpBars[i].gameObject.SetActive(false);
                _hpBars.RemoveAt(i);
            }
        }

        // �Ÿ��� ������ ���� ����Ʈ
        List<HealthBarDistanceInfo> healthBarInfos = new List<HealthBarDistanceInfo>();

        Vector3 cameraPos = _mainCamera.transform.position;

        // �� ü�¹��� �Ÿ��� ���ü� ���
        foreach (var hpBar in _hpBars)
        {
            Enemy enemy = hpBar.GetEnemy();
            if (enemy == null) continue;

            Vector3 enemyPos = enemy.transform.position;
            float distance = Vector3.Distance(cameraPos, enemyPos);

            // �⺻ ���ü� üũ
            bool isVisible = CheckVisibility(enemyPos, distance);

            healthBarInfos.Add(new HealthBarDistanceInfo
            {
                hpBar = hpBar,
                distance = distance,
                isVisible = isVisible
            });
        }

        // �Ÿ� ������ ���� (����� �ͺ���)
        healthBarInfos.Sort((a, b) => a.distance.CompareTo(b.distance));

        // �ִ� ǥ�� ���� ���� ����
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
        // 1. �Ÿ� üũ
        if (distance > maxDistance) return false;

        // 2. ī�޶� ���� �� ȭ�� �� üũ
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

        // ī�޶� ����
        if (screenPos.z < 0) return false;

        // ȭ�� �� (������ ����)
        float margin = 100f;
        if (screenPos.x < -margin || screenPos.x > Screen.width + margin ||
            screenPos.y < -margin || screenPos.y > Screen.height + margin)
            return false;

        return true;
    }

    // ��� ü�¹� ���� ������Ʈ
    public void ForceUpdateAll()
    {
        UpdateAllHealthBars();
    }

    // Ư�� ü�¹ٸ� ������Ʈ
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

    // ��ü ü�¹� �ý��� ��Ȱ��ȭ
    public void DeActivateAllHpBars()
    {
        foreach (var hpBar in _hpBars)
        {
            if (hpBar != null)
                hpBar.gameObject.SetActive(false);
        }
    }

    // ��ü ü�¹� �ý��� Ȱ��ȭ
    public void ActivateAllHpBars()
    {
        ForceUpdateAll();
    }

    // ����� ����
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

