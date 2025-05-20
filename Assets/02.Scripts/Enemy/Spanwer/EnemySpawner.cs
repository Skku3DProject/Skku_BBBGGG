using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    // 웨이브 별로 지정된 갯수, 종류 소환

    public List<GameObject> PreSetList; 
    
    private List<Enemy> _enemys;
    public List<Enemy> enemy => _enemys;

    public EnemySpawnerSo SpawnerSo;

    public int MaxSpawnCount = 10;

    private void Start()
    {
        MaxSpawnCount = MaxSpawnCount * PreSetList.Count;

        UI_Enemy.Instance.HPBarPooling(MaxSpawnCount);

        _enemys = new List<Enemy>(MaxSpawnCount);

        for (int i = 0; i < PreSetList.Count; i++)
        {
            for (int j = 0; j < MaxSpawnCount; j++)
            {
                GameObject enemyObject = Instantiate(PreSetList[i], transform);
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                enemy.Initialize();
                enemy.gameObject.SetActive(false);
                UI_Enemy.Instance.SetHpBarToEnemy(enemy);
                _enemys.Add(enemy);
            }
        }
    }

    private void Awake()
    {
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Spawn();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
                Initialize();
        }

        if(Input.GetMouseButtonDown(0))
        {
            foreach(Enemy enemy in _enemys)
            {
                Damage damage = new Damage(10, gameObject, 100);
                enemy.GetComponent<EnemyController>().TakeDamage(damage);
            }
        }
    }

    public void Spawn()
    {
        // 타입 별 for문
        for(int enemyType = 0; enemyType < (int)SpawnerSo.EnemyTypes.Count; enemyType++)
        {
            // 소환 갯수
            for(int spawnCount = 0; spawnCount < (int)SpawnerSo.SpawnCounts[enemyType]; spawnCount++ )
            {
                // 타입과 공격 타입 이 같고 비활성화된 애들만 소환
                foreach(Enemy enemy in _enemys)
                {
                    if(enemy.isActiveAndEnabled == false && enemy.EnemyData.EnemyAttackType == SpawnerSo.EnemyAttackTypes[enemyType] 
                        && enemy.EnemyData.EnemyType == SpawnerSo.EnemyTypes[enemyType])
                    {
                        enemy.GetComponent<EnemyController>().Initialize();
                        enemy.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
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
