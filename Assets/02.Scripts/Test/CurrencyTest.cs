using System.Collections.Generic;
using UnityEngine;

public class CurrencyTest : MonoBehaviour
{
    public void GoldAdd()
    {
        CurrencyManager.instance.Add(ECurrencyType.Gold, 20);
    }
    
    public void WoodAdd()
    {
        CurrencyManager.instance.Add(ECurrencyType.Wood, 20);
    }
    
    public void StoneAdd()
    {
        CurrencyManager.instance.Add(ECurrencyType.Stone, 20);
    }

    public void GoldSpend()
    {
        CurrencyManager.instance.Spend(ECurrencyType.Gold, 20);
    }

    public void MulSpend()
    {
        Dictionary<ECurrencyType,int> cost = new Dictionary<ECurrencyType,int>();
        cost.Add(ECurrencyType.Gold,20);
        cost.Add(ECurrencyType.Wood,20);
        
        CurrencyManager.instance.MultiSpend(cost);
    }
}
