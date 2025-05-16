using System;
using System.Collections.Generic;
using UnityEngine;

public enum ECurrencyType
{
    Gold,
    Wood,
    Stone,
    
    Count
}
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    private Dictionary<ECurrencyType, int> _currencies = new Dictionary<ECurrencyType, int>(); 
    //싱글톤 시작시 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        InitializeCurrency();
    }

    // 시작시 재화 초기화
    public void InitializeCurrency()
    {
        for (int i = 0; i < (int)ECurrencyType.Count; i++)
        {
            _currencies[(ECurrencyType)i] = 0;
        }
    }
    // 재화가 있음을 확인
    private bool Have(ECurrencyType currency, int amount)
    {
        return _currencies[currency] >= amount;
    }
    // 재화 추가
    public void Add(ECurrencyType currency, int amount)
    {
        _currencies[currency] += amount;
        UIManager.instance.RefreshCurrnecy(currency,_currencies[currency]);
    }
    
    // 재화 사용 (제거)
    public bool Spend(ECurrencyType currency, int amount)
    {
        if (!Have(currency, amount))
        {
            return false;    
        }
        _currencies[currency] -= amount;
        UIManager.instance.RefreshCurrnecy(currency,_currencies[currency]);
        return true;
    }
    // 여러 재화를 한번에 사용한다. => new dictionary 선언해서 사용해야함
    public bool MultiSpend(Dictionary<ECurrencyType, int> currencies)
    {
        foreach (var cost in currencies)
        {
            if (!Have(cost.Key, cost.Value))
            {
                return false;
            }
        }

        foreach (var cost in currencies)
        {
            _currencies[cost.Key] -= cost.Value;
            UIManager.instance.RefreshCurrnecy(cost.Key,_currencies[cost.Key]);
        }
        
        return true;
    }
}
