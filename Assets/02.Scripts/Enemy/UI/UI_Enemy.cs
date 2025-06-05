using System.Collections.Generic;
using UnityEngine;
public class UI_Enemy : MonoBehaviour
{
    public static UI_Enemy Instance = null;

    public float maxDistance = 50f; // 최대 표시 거리
    public int maxVisibleHealthBars = 30; // 최대 표시 개수
    public int maxVisibleText = 30; // 최대 표시 개수

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
        _AcitveHpBars = new List<UI_EnemyHpbar>(capacity); // 보이는
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
            // 거리 가시성 체크
            bool isVisible = CheckVisibility(enemyPos, distance) && visibleCount < maxVisibleHealthBars;
            hpBar.gameObject.SetActive(isVisible);
            if (isVisible)
            {
                visibleCount++;
            }
         
        }
      
    }
    // 전부 끝났을때 한번만 실행
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
        // 1. 거리 체크
        if (distance > maxDistance) return false;
        // 2. 카메라 뒤쪽 및 화면 밖 체크
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);
        // 카메라 뒤쪽
        if (screenPos.z < 0) return false;
        // 화면 밖 (여유분 포함)
        float margin = 100f;
        if (screenPos.x < -margin || screenPos.x > Screen.width + margin || screenPos.y < -margin || screenPos.y > Screen.height + margin)
            return false;

        return true;
    }

}