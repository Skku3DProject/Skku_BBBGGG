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
    
    private List<int> _currencies = new List<int>((int)ECurrencyType.Count);
    
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
            _currencies.Add(0);
        }
    }
    // 재화가 있음을 확인
    private bool Have(ECurrencyType currency, int amount)
    {
        return _currencies[(int)currency] >= amount;
    }
    // 
    // 재화 추가
    public void Add(ECurrencyType currency, int amount)
    {
        _currencies[(int)currency] += amount;
        UIManager.instance.RefreshCurrnecy(currency,_currencies[(int)currency]);
    }
    // 재화 사용 (제거)
    public void Spend(ECurrencyType currency, int amount)
    {
        if (!Have(currency, amount))
        {
            return;    
        }
        _currencies[(int)currency] -= amount;
        UIManager.instance.RefreshCurrnecy(currency,_currencies[(int)currency]);
    }
     
    
}
