using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI[] Currencies;
    public GameObject GameOverPanel;
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
    public void RefreshCurrency()
    {
        Currencies[0].text = $"{CurrencyManager.instance.CurrentCurrency().Gold}";
        Currencies[1].text = $"{CurrencyManager.instance.CurrentCurrency().Wood}";
        Currencies[2].text = $"{CurrencyManager.instance.CurrentCurrency().Stone}";
    }
    public void UI_GameOver()
    {
        GameOverPanel.SetActive(true);
    }
    
}
