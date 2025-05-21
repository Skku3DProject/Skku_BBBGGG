using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI[] Currencies;
    //public GameObject GameOverPanel;
    
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI Stage;
    public TextMeshProUGUI Phase;
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
        Currencies[0].text = $"Gold :{CurrencyManager.instance.CurrentCurrency().Gold}";
        Currencies[1].text = $"Wood :{CurrencyManager.instance.CurrentCurrency().Wood}";
        Currencies[2].text = $"Stone :{CurrencyManager.instance.CurrentCurrency().Stone}";
    }
    public void UI_GameOver()
    {
      //  GameOverPanel.SetActive(true);
    }

    public void UI_TimerRefresh(float time)
    {
        Timer.text = $"{(int)time}";
    }

    public void UI_Stage(EStageType stage)
    {
        Stage.text = stage.ToString();
    }

    public void UI_Phase(EPhaseType phase)
    {
        Phase.text = phase.ToString();
    }
}
