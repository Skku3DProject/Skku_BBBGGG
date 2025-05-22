using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 웨이브 별로 지정된 갯수, 종류 소환

    public List<GameObject> PreSetList;

    private List<Enemy> _enemys;
    public List<Enemy> enemy => _enemys;

    public List<So_EnemySpawner> SpawnerSo;

    public int MaxSpawnCount = 10;

    private void Awake()
    {


    }
    private void Start()
    {
        MaxSpawnCount = MaxSpawnCount * PreSetList.Count;
        UI_Enemy.Instance.SetHPBarMaxSize(MaxSpawnCount);

        Pool();

        StageManager.instance.OnCombatStart += Spawn;

    }

    private void Update()
    {

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Spawn();
        //}

        if (Input.GetKeyDown(KeyCode.L))
        {
            Initialize();
        }

        //if(Input.GetMouseButtonDown(0))
        //{
        //    foreach(Enemy enemy in _enemys)
        //    {
        //        Damage damage = new Damage(10, gameObject, 100);
        //        enemy.GetComponent<EnemyController>().TakeDamage(damage);
        //    }
        //}
    }

    private void Pool()
    {
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
    public void Spawn()
    {
  
        // 현재 데이터를 읽는다.
        int CurrentStage = (int)StageManager.instance.GetCurrentStage();
        for (int spawnCountIndex = 0; spawnCountIndex < SpawnerSo[CurrentStage].SpawnCounts.Count; spawnCountIndex++)
        {
            for (int spawnCount = 0; spawnCount < SpawnerSo[CurrentStage].SpawnCounts[spawnCountIndex]; spawnCount++)
            {
                foreach (Enemy enemy in _enemys)
                {

                    if (SpawnerSo[CurrentStage].EnemyTypes[spawnCountIndex].name + "(Clone)" == enemy.name && enemy.isActiveAndEnabled == false)
                    {
                        enemy.Initialize(); // Enemy 초기화
                        enemy.GetComponent<EnemyController>().Initialize(); // FSM 초기화
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
