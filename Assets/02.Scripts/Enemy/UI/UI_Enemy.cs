using System.Collections.Generic;
using UnityEngine;
public class UI_Enemy : MonoBehaviour
{
    public static UI_Enemy Instance = null;

    public float maxDistance = 50f; // �ִ� ǥ�� �Ÿ�
    public int maxVisibleHealthBars = 30; // �ִ� ǥ�� ����
    public int maxVisibleText = 30; // �ִ� ǥ�� ����

    private List<UI_EnemyHpbar> _AcitveHpBars;

    private Camera _mainCamera;
    private List<HealthBarDistanceInfo> _healthBarInfos;

    private EnemyUIPoolManager _enemyUIPoolManager;

    public string HealthBarKey;
    public string DamageTextKey;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _mainCamera = Camera.main;
        _enemyUIPoolManager = GetComponent<EnemyUIPoolManager>();
    }
    private void Update()
    {
        if (_healthBarInfos != null)
        {
            UpdateAllHealthBars();
        }
    }

    public void SetHPBarMaxSize(int capacity)
    {
        _AcitveHpBars = new List<UI_EnemyHpbar>(capacity); // ���̴�
    }
    public UI_EnemyHpbar SetHpBarToEnemy(Enemy enemy)
    {
        GameObject hpBar = _enemyUIPoolManager.GetObject(HealthBarKey);
        UI_EnemyHpbar hpBarComponent = hpBar.GetComponent<UI_EnemyHpbar>();
        hpBarComponent.SetHpBarToEnemy(enemy);
        _AcitveHpBars.Add(hpBarComponent);
        if (hpBarComponent == null)
        {
            return null;
        }
        return hpBarComponent;
    }
    public void TurnOffHpBar(Enemy enemy)
    {
        foreach (UI_EnemyHpbar hpbar in _AcitveHpBars)
        {
            if (hpbar.GetEnemy() == enemy)
            {
                _enemyUIPoolManager.ReturnObject(HealthBarKey, hpbar.gameObject);
                _AcitveHpBars.Remove(hpbar);
                break;
            }
        }
    }
    private void UpdateAllHealthBars()
    {
        _healthBarInfos.Clear();
        Vector3 cameraPos = _mainCamera.transform.position;
      
        foreach (var hpBar in _AcitveHpBars)
        {
            Enemy enemy = hpBar.GetEnemy();
            if (enemy == null || enemy.gameObject.activeInHierarchy == false) continue;
            Vector3 enemyPos = enemy.transform.position;
            float distance = Vector3.Distance(cameraPos, enemyPos);
            // �⺻ ���ü� üũ
            bool isVisible = CheckVisibility(enemyPos, distance);
            _healthBarInfos.Add(new HealthBarDistanceInfo
            {
                hpBar = hpBar,
                distance = distance,
                isVisible = isVisible
            });
        }
        // �Ÿ� ������ ���� (����� �ͺ���)
        _healthBarInfos.Sort((a, b) => a.distance.CompareTo(b.distance));
        // �ִ� ǥ�� ���� ���� ����
        int visibleCount = 0;
        foreach (var info in _healthBarInfos)
        {
            bool shouldShow = info.isVisible && visibleCount < maxVisibleHealthBars;
            info.hpBar.gameObject.SetActive(shouldShow);
            if (shouldShow)
            {
                visibleCount++;
            }
        }
    }
    // ���� �������� �ѹ��� ����
    public void UpdateHealthBars()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }
        // �Ÿ��� ������ ���� ����Ʈ
        _healthBarInfos = new List<HealthBarDistanceInfo>(maxVisibleHealthBars);
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
}