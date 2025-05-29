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
        SetSpawnPosition();
      

        StartCoroutine(Spawn_Coroutine());
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

    private IEnumerator Spawn_Coroutine()
    {
        int i = 0;
        int currentStage = (int)StageManager.instance.GetCurrentStage();
        // 현재 스테이지에 소환 데이터
        So_EnemySpawner currentSo = SpawnerSo[currentStage];
        
        // 스폰할 몬스터 양 가져오기
        foreach(EnemySpawnContainer enemySpawnContainer in currentSo.SpawnEneies)
        {
            // 소환중인 몬스터의 번호
            for(int index=0; index< enemySpawnContainer.SpawnCounts; index++)
            {
                // 현재 소환 몬스터 인덱스
                GameObject enemy = EnemyPoolManager.Instance.GetObject(enemySpawnContainer.So_Enemy.Key);
                enemy.transform.position = _positionList[i++];
                yield return new WaitForSeconds(SpawnTime);
            }
        }
      
        UI_Enemy.Instance.UpdateHealthBars();
        UIManager.instance.CurrentCountRefresh();
        yield break;
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
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        float y = transform.position.y; // 지면 높이
        return new Vector3(x, y, z);
    }

}
