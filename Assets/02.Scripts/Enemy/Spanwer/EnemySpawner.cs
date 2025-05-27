using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class EnemySpawner : MonoBehaviour
{
    // 웨이브 별로 지정된 갯수, 종류 소환

    public List<GameObject> PreSetList;

    private List<Enemy> _enemys;
    public List<Enemy> enemy => _enemys;

    public List<So_EnemySpawner> SpawnerSo;

    public float SpawnTime = 0.3f;
    
    public int MaxSpawnCount = 10;

    private float _minDistance = 1.0f; // 최소 거리 설정 

    private List<GameObject> _positionList;
   

    private void Start()
    {
        MaxSpawnCount = MaxSpawnCount * PreSetList.Count;
        EnemyManager.Instance.SetEnemiesList(MaxSpawnCount);
        UI_Enemy.Instance.SetHPBarMaxSize(MaxSpawnCount);
        _positionList = new List<GameObject>(MaxSpawnCount);

        EnemyPool();

        StageManager.instance.OnCombatStart += Spawn;

    }

    private void EnemyPool()
    {
        _enemys = new List<Enemy>(MaxSpawnCount);
        for (int i = 0; i < PreSetList.Count; i++)
        {
            for (int j = 0; j < MaxSpawnCount; j++)
            {
                GameObject enemyObject = Instantiate(PreSetList[i], transform);
                Enemy enemy = enemyObject.GetComponent<Enemy>();

                EnemyProjectilePool(enemy);
                enemy.gameObject.SetActive(false);
                UI_Enemy.Instance.SetHpBarToEnemy(enemy);
                _enemys.Add(enemy);
            }
        }
	}

    private void EnemyProjectilePool(Enemy enemy)
    {
        if (enemy.EnemyData.ProjectilePrefab == null)
        {
            return;
        }

        List<GameObject> prefabs = new List<GameObject>(enemy.EnemyData.PrefabSize);

        for(int i=0; i< enemy.EnemyData.PrefabSize; i++)
        {
            GameObject prefab = Instantiate(enemy.EnemyData.ProjectilePrefab, enemy.ProjectileTransfrom);
            prefab.GetComponent<EnemyProjectile>().SetParentTranfrom( enemy.ProjectileTransfrom) ;
            prefab.SetActive(false);
            prefabs.Add(prefab); 
        }

        enemy.gameObject.GetComponent<EnemyAttackCheckEvent>().ProjectilePrefabs = prefabs;
    }
    public void Spawn()
    {
        StartCoroutine(Spawn_Coroutine());
    }

    private IEnumerator Spawn_Coroutine()
    {
        int currentStage = (int)StageManager.instance.GetCurrentStage();
        _positionList.Clear(); // 기존 위치 초기화

        for (int spawnCountIndex = 0; spawnCountIndex < SpawnerSo[currentStage].SpawnCounts.Count; spawnCountIndex++)
        {
            int spawnCount = SpawnerSo[currentStage].SpawnCounts[spawnCountIndex];
            string targetName = SpawnerSo[currentStage].EnemyTypes[spawnCountIndex].name + "(Clone)";

            for (int spawnCountProgress = 0; spawnCountProgress < spawnCount; spawnCountProgress++)
            {
                Enemy enemy = GetInactiveEnemy(targetName);
                if (enemy != null)
                {
                    _minDistance = enemy.CharacterController.radius;
                    Vector3 spawnPos = GetValidSpawnPosition();
                    enemy.transform.position = spawnPos;
                    InitializeEnemy(enemy);
                    _positionList.Add(enemy.gameObject);
                }
                yield return new WaitForSeconds(SpawnTime);
            }
        }
        UI_Enemy.Instance.UpdateHealthBars();
        UIManager.instance.CurrentCountRefresh();
        yield break;
    }
    private Enemy GetInactiveEnemy(string targetName)
    {
        foreach (Enemy enemy in _enemys)
        {
            if (enemy.name == targetName && !enemy.gameObject.activeSelf)
            {
                return enemy;
            }
        }
        return null;
    }
    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnOffset;
        int maxAttempts = 50;
        int attempts = 0;

        do
        {
            spawnOffset = GetRandomSpawnPosition();
            attempts++;
        }
        while (!IsValidPosition(transform.position + spawnOffset) && attempts < maxAttempts);

        return transform.position + spawnOffset;
    }
    private bool IsValidPosition(Vector3 position)
    {
        foreach (GameObject obj in _positionList)
        {
            if (Vector3.Distance(obj.transform.position, position) < _minDistance)
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
    private void InitializeEnemy(Enemy enemy)
    {
        enemy.Initialize();
        enemy.GetComponent<EnemyController>().Initialize();
        EnemyManager.Instance.Enable(enemy);
    }

    private void Initialize()
    {
        foreach (Enemy enemy in _enemys)
        {
            enemy.gameObject.SetActive(false);
            enemy.transform.position = transform.position;
        }
    }
}
