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

    //private float _minDistance = 1.0f; // 최소 거리 설정 

    private List<GameObject> _positionList;

    private void Start()
    {
        MaxSpawnCount = EnemyPoolManager.Instance.MaxEnemyCount;

        EnemyManager.Instance.SetEnemiesList(MaxSpawnCount);
        UI_Enemy.Instance.SetHPBarMaxSize(MaxSpawnCount);
        StageManager.instance.OnCombatStart += Spawn;

        _positionList = new List<GameObject>(MaxSpawnCount);
    }

    public void Spawn()
    {
        StartCoroutine(Spawn_Coroutine());
    }

    private IEnumerator Spawn_Coroutine()
    {
        int currentStage = (int)StageManager.instance.GetCurrentStage();
        So_EnemySpawner currentSo = SpawnerSo[currentStage];

        foreach (EnemySpawnContainer enemySpawnContainer in currentSo.SpawnEneies)
        {
            for (int i = 0; i < enemySpawnContainer.SpawnCounts; i++)
            {
                if (EnemyPoolManager.Instance.GetObject(enemySpawnContainer.So_Enemy.Key).TryGetComponent<Enemy>(out var enemy))
                {
                    if (enemy.CharacterController == null)
                    {
                        Debug.Log(enemy.name);
                        continue;
                    }
                    float _minDistance = enemy.CharacterController.radius;
                    _positionList.Add(enemy.gameObject);
                    Vector3 spawnPos = GetValidSpawnPosition(_minDistance);
                    enemy.transform.position = spawnPos;
                }
                yield return new WaitForSeconds(SpawnTime);
            }
        }

        _positionList.Clear(); // 기존 위치 초기화
        UI_Enemy.Instance.UpdateHealthBars();
        UIManager.instance.CurrentCountRefresh();
        yield break;
    }

    private Vector3 GetValidSpawnPosition( float minDistance)
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
            return Vector3.zero;
        }

        return transform.position + spawnOffset;
    }
    private bool IsValidPosition(Vector3 position,float minDistance)
    {
        foreach (GameObject obj in _positionList)
        {
            if (Vector3.Distance(obj.transform.position, position) < minDistance)
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
