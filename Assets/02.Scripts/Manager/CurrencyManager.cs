using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    private Currency _currentCurrency;
    
    //싱글톤 시작시 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        Initialize();

        StageManager.instance.OnCombatEnd += ClearStage;    }

    private void ClearStage()
    {
        _currentCurrency.Gold += 3000;
        UIManager.instance.RefreshCurrency();
    }

    private void Initialize()
    {
        _currentCurrency = new Currency()
        {
            Gold = 5000, Wood = 3000, Stone = 3000
        };
        
        UIManager.instance.RefreshCurrency();
    }
    // 재화가 있음을 확인
    public bool Have(Currency required)
    {
        return _currentCurrency.Gold >= required.Gold &&
               _currentCurrency.Stone >= required.Stone &&
               _currentCurrency.Wood >= required.Wood;
    }
    // 재화 추가 
    public void Add(Currency currency)
    {
        _currentCurrency.Gold += currency.Gold;
        _currentCurrency.Wood += currency.Wood;
        _currentCurrency.Stone += currency.Stone;
        
        UIManager.instance.RefreshCurrency();
       // 튜토리얼일 경우 튜토리얼 퀘스트를 깬다.
        if (StageManager.instance.GetCurrentStage() != EStageType.Tutorial)
        {
            return;
        }
        var currentTutorial = TutorialManager.Instance.CurrentTutorial;
        if (currentTutorial.Type == TutorialType.CollectWood && currency.Wood > 0)
        {
            TutorialEvent.OnProgress?.Invoke(TutorialType.CollectWood,1);
            return;
        }
        if (currentTutorial.Type == TutorialType.CollectRock&& currency.Stone > 0)
        {
            TutorialEvent.OnProgress?.Invoke(TutorialType.CollectRock,1);
        }
    }
    
    // 재화 사용 - 재화 하나 사용하기 (제거)
    public bool Spend(Currency currency)
    {
        if (!Have(currency))
        {
            Debug.Log("재화가 모자랍니다.");
            return false;
        }
        _currentCurrency.Gold -= currency.Gold;
        _currentCurrency.Wood -= currency.Wood;
        _currentCurrency.Stone -= currency.Stone;
        UIManager.instance.RefreshCurrency();
        
        if (StageManager.instance.GetCurrentStage() == EStageType.Tutorial)
        {
            var currentTutorial = TutorialManager.Instance.CurrentTutorial;
            if (currentTutorial.Type == TutorialType.CreateTower)
            {
                TutorialEvent.OnProgress?.Invoke(TutorialType.CreateTower,1);
            }
            return true;
        }
        
        return true;
    }
    public Currency CurrentCurrency()
    {
        return _currentCurrency;
    }

}
