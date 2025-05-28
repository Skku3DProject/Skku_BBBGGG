using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class EnemySpawner : MonoBehaviour
{
    private List<Enemy> _enemys;
    public List<Enemy> enemy => _enemys;

    public List<So_EnemySpawner> SpawnerSo;

    public float SpawnTime = 0.3f;
    
    public int MaxSpawnCount = 10;

    private float _minDistance = 1.0f; // 최소 거리 설정 

    private List<GameObject> _positionList;
   
    private void Start()
    {
        MaxSpawnCount = EnemyPoolManager.Instance.MaxEnemyCount;

        EnemyManager.Instance.SetEnemiesList(MaxSpawnCount);
        UI_Enemy.Instance.SetHPBarMaxSize(MaxSpawnCount);
        StageManager.instance.OnCombatStart += Spawn;

        _positionList = new List<GameObject>(MaxSpawnCount);
    }

    private void EnemyPool()
    {
        _enemys = new List<Enemy>(MaxSpawnCount);
        //for (int i = 0; i < PreSetList.Count; i++)
        //{
        //    for (int j = 0; j < MaxSpawnCount; j++)
        //    {
     
        //        
        //        _enemys.Add(enemy);
        //    }
        //}
	}

    private void EnemyProjectilePool(Enemy enemy)
    {
        //if (enemy.EnemyData.ProjectilePrefab == null)
        //{
        //    return;
        //}

        //List<GameObject> prefabs = new List<GameObject>(enemy.EnemyData.PrefabSize);

        //for(int i=0; i< enemy.EnemyData.PrefabSize; i++)
        //{
        //    GameObject prefab = Instantiate(enemy.EnemyData.ProjectilePrefab, enemy.ProjectileTransfrom);
        //    prefab.GetComponent<EnemyProjectile>().SetParentTranfrom( enemy.ProjectileTransfrom) ;
        //    prefab.SetActive(false);
        //    prefabs.Add(prefab); 
        //}

        //enemy.gameObject.GetComponent<EnemyAttackCheckEvent>().ProjectilePrefabs = prefabs;
    }
    public void Spawn()
    {
        StartCoroutine(Spawn_Coroutine());
    }

    private IEnumerator Spawn_Coroutine()
    {
        int currentStage = (int)StageManager.instance.GetCurrentStage();
        So_EnemySpawner currentSo = SpawnerSo[currentStage];

        foreach(EnemySpawnContainer enemySpawnContainer in currentSo.SpawnEneies)
        {
            for (int i = 0; i < enemySpawnContainer.SpawnCounts; i++)
            {
                if (EnemyPoolManager.Instance.GetObject(enemySpawnContainer.So_Enemy.Key).TryGetComponent<Enemy>(out var enemy))
                {
                    if(enemy.CharacterController == null)
                    {
                        Debug.Log(enemy.name);
                        continue;
                    }
                    _minDistance = enemy.CharacterController.radius;
                    Vector3 spawnPos = GetValidSpawnPosition();
                    enemy.transform.position = spawnPos;
                    _positionList.Add(enemy.gameObject);
                }
                yield return new WaitForSeconds(SpawnTime);
            }
        }

        _positionList.Clear(); // 기존 위치 초기화
        UI_Enemy.Instance.UpdateHealthBars();
        UIManager.instance.CurrentCountRefresh();
        yield break;
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
    
}
