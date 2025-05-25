using System.Collections.Generic;
using UnityEngine;

public class UI_Enemy : MonoBehaviour
{
	public static UI_Enemy Instance = null;

	public GameObject HPbar;

	public float maxDistance = 50f; // 최대 표시 거리
	public int maxVisibleHealthBars = 30; // 최대 표시 개수

	private List<UI_EnemyHpbar> _hpBars;
	private List<UI_EnemyHpbar> _AcitveHpBars;
	private Camera _mainCamera;

	private List<HealthBarDistanceInfo> _healthBarInfos;


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
		if ((_healthBarInfos != null))
		{
			UpdateAllHealthBars();
		}
	}

	public void SetHPBarMaxSize(int capacity)
	{
		_hpBars = new List<UI_EnemyHpbar>(capacity);
		_AcitveHpBars = new List<UI_EnemyHpbar>(capacity);
	}


	public void SetHpBarToEnemy(Enemy enemy)
	{
		GameObject hpBar = Instantiate(HPbar, transform);
		UI_EnemyHpbar hpBarComponent = hpBar.GetComponent<UI_EnemyHpbar>();

		hpBar.SetActive(false);
		hpBarComponent.SetHpBarToEnemy(enemy);
		_hpBars.Add(hpBarComponent);
	}
	public void TurnOffHpBar(Enemy enemy)
	{
		foreach (UI_EnemyHpbar hpbar in _AcitveHpBars)
		{
			if (hpbar.GetEnemy() == enemy)
			{
				hpbar.gameObject.SetActive(false);
				_AcitveHpBars.Remove(hpbar);
				break;
			}
		}
	}
	private void UpdateAllHealthBars()
	{
		_healthBarInfos.Clear();
		Debug.Log(_healthBarInfos.Count);

		Vector3 cameraPos = _mainCamera.transform.position;

		// 1._hpBars 중에서 켜져있는애들을 저장
		// 2. 켜져 있는 애들끼리 거리값 비교
		// 3. 
		// 각 체력바의 거리와 가시성 계산
		foreach (var hpBar in _AcitveHpBars)
		{
			Enemy enemy = hpBar.GetEnemy();

			if (enemy == null || enemy.gameObject.activeInHierarchy == false) continue;

			Vector3 enemyPos = enemy.transform.position;
			float distance = Vector3.Distance(cameraPos, enemyPos);

			// 기본 가시성 체크
			bool isVisible = CheckVisibility(enemyPos, distance);

			_healthBarInfos.Add(new HealthBarDistanceInfo
			{
				hpBar = hpBar,
				distance = distance,
				isVisible = isVisible
			});
		}

		// 거리 순으로 정렬 (가까운 것부터)
		_healthBarInfos.Sort((a, b) => a.distance.CompareTo(b.distance));

		// 최대 표시 개수 제한 적용
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

	// 전부 끝났을때 한번만 실행
	public void UpdateHealthBars()
	{
		if (_mainCamera == null)
		{
			_mainCamera = Camera.main;
			if (_mainCamera == null) return;
		}

		foreach (UI_EnemyHpbar hpBar in _hpBars)
		{
			if (hpBar.gameObject.activeInHierarchy && hpBar.GetEnemy() != null)
			{
				_AcitveHpBars.Add(hpBar);
			}
		}

		// 거리별 정렬을 위한 리스트
		_healthBarInfos = new List<HealthBarDistanceInfo>(_AcitveHpBars.Count);
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

