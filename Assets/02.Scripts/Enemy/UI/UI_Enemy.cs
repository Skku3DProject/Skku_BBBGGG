using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UI_Enemy : MonoBehaviour
{
    public static UI_Enemy Instance = null;

    public GameObject HPbar;

    private List<GameObject> _hpBars;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    public void SetHPBarMaxSize(int capacity)
    {
        _hpBars = new List<GameObject>(capacity);
    }

    public void SetHpBarToEnemy(Enemy enemy)
    {
        GameObject hpBar = Instantiate(HPbar, transform);
        hpBar.SetActive(false);
        hpBar.GetComponent<UI_EnemyHpbar>().SetHpBarToEnemy(enemy);
        _hpBars.Add(hpBar);
    }

    // 추후 최적화시 사용
    public void DeActivateHpBar()
    {
        gameObject.SetActive(false);
    }
}
