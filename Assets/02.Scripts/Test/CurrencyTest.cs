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
}
