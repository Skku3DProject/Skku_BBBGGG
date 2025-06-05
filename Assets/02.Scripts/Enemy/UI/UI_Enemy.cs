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
        UpdateAllHealthBars();
    }

    public void SetHPBarMaxSize(int capacity)
    {
        _AcitveHpBars = new List<UI_EnemyHpbar>(capacity); // ���̴�
    }
    public UI_EnemyHpbar SetHpBarToEnemy(Enemy enemy)
    {
        GameObject hpBar = _enemyUIPoolManager.GetObject(HealthBarKey);
        hpBar.SetActive(false);
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

    public void UpdateDamageText(Damage damage, Enemy enemy)
    {
        UI_EnemyDamageText damageText = _enemyUIPoolManager.GetObject(DamageTextKey).GetComponent<UI_EnemyDamageText>();
        damageText.gameObject.SetActive(false);
        if (damageText == null) return;
        damageText.UpdateText(damage, enemy);
    }

    public void CreaDamageText(GameObject damageText)
    {
        _enemyUIPoolManager.ReturnObject(DamageTextKey, damageText);
    }

    private void UpdateAllHealthBars()
    {
        Vector3 cameraPos = _mainCamera.transform.position;
        int visibleCount = 0;

        if (_AcitveHpBars.Count <= 0) return;

        foreach (var hpBar in _AcitveHpBars)
        {
            Enemy enemy = hpBar.GetEnemy();
            if (enemy == null || enemy.gameObject.activeInHierarchy == false) continue;
            Vector3 enemyPos = enemy.transform.position;
            float distance = Vector3.Distance(cameraPos, enemyPos);
            // �Ÿ� ���ü� üũ
            bool isVisible = CheckVisibility(enemyPos, distance) && visibleCount < maxVisibleHealthBars;
            hpBar.gameObject.SetActive(isVisible);
            if (isVisible)
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
        if (screenPos.x < -margin || screenPos.x > Screen.width + margin || screenPos.y < -margin || screenPos.y > Screen.height + margin)
            return false;

        return true;
    }

}