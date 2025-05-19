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
    public void RefreshCurrnecy(ECurrencyType currency , int value)
    {
        Currencies[(int)currency].text = $"{value}";
    }

    public void UI_GameOver()
    {
        GameOverPanel.SetActive(true);
    }
    
}
