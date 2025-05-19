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

    // 재화 종류
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
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        InitializeCurrency();
    }

    // 시작시 재화 초기화 -> 시작 재화 정하기, 0으로 초기화
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
    
    // 재화 사용 - 재화 하나 사용하기 (제거)
    public bool Spend(ECurrencyType currency, int amount)
    {
        if (!Have(currency, amount))
        {
            Debug.Log($"{currency.ToString()} 가 부족합니다.");
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
                Debug.Log($"{cost.Key} 가 모자랍니다.");
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
