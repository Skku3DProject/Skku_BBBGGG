using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI[] Currencies;
    // 싱글톤
    public void Awake()
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

    // 재화 변경
    public void RefreshCurrnecy(ECurrencyType currency , int value)
    {
        Currencies[(int)currency].text = $"{value}";
    }
    
    
}
