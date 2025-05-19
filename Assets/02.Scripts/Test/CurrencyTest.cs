using System.Collections.Generic;
using UnityEngine;

public class CurrencyTest : MonoBehaviour
{
    public void GoldAdd()
    {
        Currency currency = new Currency() {Gold = 20};
        CurrencyManager.instance.Add(currency);
    }
    
    public void WoodAdd()
    {
        Currency currency = new Currency() {Wood = 20};
        CurrencyManager.instance.Add(currency);
        
    }
    
    public void StoneAdd()
    {
        Currency currency = new Currency() {Stone = 20};
        CurrencyManager.instance.Add(currency);
    }

    public void GoldSpend()
    {
        Currency currency = new Currency(){Wood = 20, Stone = 10};
        CurrencyManager.instance.Spend(currency);
    }


    // public void MulSpend()
    // {
    //     Dictionary<ECurrencyType,int> cost = new Dictionary<ECurrencyType,int>();
    //     cost.Add(ECurrencyType.Gold,20);
    //     cost.Add(ECurrencyType.Wood,20);
    //     
    //     CurrencyManager.instance.MultiSpend(cost);
    // }
}
