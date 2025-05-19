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

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        _currentCurrency = new Currency()
        {
            Gold = 0, Wood = 0, Stone = 0
        };
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
        
        return true;
    }
    public Currency CurrentCurrency()
    {
        return _currentCurrency;
    }

}
