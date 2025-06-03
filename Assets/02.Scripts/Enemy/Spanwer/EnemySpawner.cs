using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private List<Enemy> _enemys;
    public List<Enemy> enemy => _enemys;

    public List<So_EnemySpawner> SpawnerSo;

    public float SpawnTime = 0.3f;

    public int MaxSpawnCount = 10;

    private List<Vector3> _positionList;

    private So_EnemySpawner currentSo;

    private float spawnTimer = 0f;
    private int spawnIndex = 0;
    private bool isSpawning = false;

    private List<GameObject> spawnEnemyPrefabs = new List<GameObject>();


    private void Start()
    {
        MaxSpawnCount = EnemyPoolManager.Instance.MaxEnemyCount;

        EnemyManager.Instance.SetEnemiesList(MaxSpawnCount);
        UI_Enemy.Instance.SetHPBarMaxSize(MaxSpawnCount);
        StageManager.instance.OnCombatStart += Spawn;

        _positionList = new List<Vector3>(MaxSpawnCount);
    }

    public void Spawn()
    {
        EnemyManager.Instance.SummonEnemiesClear();

		SetSpawnPosition();

        int currentStage = (int)StageManager.instance.GetCurrentStage();
        currentSo = SpawnerSo[currentStage];

        spawnEnemyPrefabs.Clear();
        spawnIndex = 0;
        spawnTimer = 0f;

        // 몬스터 프리팹 풀링에서 미리 받아옴
        foreach (EnemySpawnContainer container in currentSo.SpawnEneies)
        {
            for (int i = 0; i < container.SpawnCounts; i++)
            {
                GameObject enemy = EnemyPoolManager.Instance.GetObject(container.So_Enemy.Key);
                enemy.SetActive(false); // 일단 꺼놓고, 위치만 배치
                spawnEnemyPrefabs.Add(enemy);
            }
        }

        isSpawning = true;
    }

    public void Update()
    {
        if (!isSpawning)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= SpawnTime && spawnIndex < spawnEnemyPrefabs.Count)
        {
            spawnTimer = 0f;

            GameObject enemy = spawnEnemyPrefabs[spawnIndex];
            enemy.transform.position = _positionList[spawnIndex];
            enemy.SetActive(true);

            spawnIndex++;

            if (spawnIndex >= spawnEnemyPrefabs.Count)
            {
                FinishSpawn();
            }
        }
    }
    private void FinishSpawn()
    {
        isSpawning = false;
        UI_Enemy.Instance.UpdateHealthBars();
        UIManager.instance.CurrentCountRefresh();
        EnemyManager.Instance.SpawnerMonsterPositionList(_positionList);
    }

    private void SetSpawnPosition()
    {
        _positionList.Clear(); // 기존 위치 초기화

        int currentStage = (int)StageManager.instance.GetCurrentStage();
        So_EnemySpawner currentSo = SpawnerSo[currentStage];

        foreach (EnemySpawnContainer enemySpawnContainer in currentSo.SpawnEneies)
        {
            for (int i = 0; i < enemySpawnContainer.SpawnCounts; i++)
            {
                float _minDistance = 2;
                _positionList.Add(GetValidSpawnPosition(_minDistance));
            }
        }
    }

    private Vector3 GetValidSpawnPosition(float minDistance)
    {
        Vector3 spawnOffset;
        int maxAttempts = 100;
        int attempts = 0;

        do
        {
            spawnOffset = GetRandomSpawnPosition();
            attempts++;
        }
        while (!IsValidPosition(transform.position + spawnOffset, minDistance) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Spawn position could not be found after max attempts");
            return Vector3.zero + spawnOffset;
        }

        return transform.position + spawnOffset;
    }
    private bool IsValidPosition(Vector3 position,float minDistance)
    {
        foreach (Vector3 obj in _positionList)
        {
            if (Vector3.Distance(obj, position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = UnityEngine.Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        float y = transform.position.y; // 지면 높이

        return new Vector3(x, y, z);
    }

}
