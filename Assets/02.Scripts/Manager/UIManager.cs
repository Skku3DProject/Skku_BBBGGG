using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Slider CurrentTime;
    public Slider HpBar;
    public Slider Mpbar;
    public Slider EXPBar;
    public TextMeshProUGUI[] Currencies;
    //public GameObject GameOverPanel;
    
    public TextMeshProUGUI Timer;
    public GameObject TimerObject;
    public GameObject CountObject;
    public TextMeshProUGUI CurrentEnemy;
    // public TextMeshProUGUI Stage;
    // public TextMeshProUGUI Phase;
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

    public void CurrentCountRefresh()
    {
        CurrentEnemy.text = $"{EnemyManager.Instance.CurruntEnemyList.Count}";
    }
    // 재화 변경
    public void RefreshCurrency()
    {
        Currencies[0].text = $"Gold :{CurrencyManager.instance.CurrentCurrency().Gold}";
        Currencies[1].text = $"Wood :{CurrencyManager.instance.CurrentCurrency().Wood}";
        Currencies[2].text = $"Stone :{CurrencyManager.instance.CurrentCurrency().Stone}";
    }
    // 게임오버
    public void UI_GameOver()
    {
      //  GameOverPanel.SetActive(true);
    }

    public void UI_ObjectOnOff(GameObject ui)
    {
        TimerObject.SetActive(false);
        CountObject.SetActive(false);
        
        ui.SetActive(true);
    }

    public void UI_PlayerSetMaxStat(float hp, float mp)
    {
        UI_PlayerSetMaxHP(hp);
        UI_PlayerSetMaxMp(mp);
    }
    // 슬라이더 맥스 밸류 정하는 메서드
    public void UI_PlayerSetMaxMp(float value) =>SetSliderValue(Mpbar, value);
    public void UI_PlayerSetMaxHP(float value) => SetSliderValue(HpBar, value);
    public void UI_SetMaxTimer(float value) => SetSliderValue(CurrentTime, value);
    
    // 슬라이더 리프레시하는 메서드
    public void UI_EXPSlider(float value) => RefreshSlider(EXPBar, value);
    public void UI_MpSlider(float value)=> RefreshSlider(Mpbar, value);
    public void UI_HpSlider(float value) => RefreshSlider(HpBar, value);
    //--------------------------------------------------------------------
    private void SetSliderValue(Slider slider, float value)
    {
        slider.maxValue = value;
        RefreshSlider(slider, value);
    }
    //슬라이더를 조절하는 함수 베이스
    private void RefreshSlider(Slider slider, float value)
    {
        slider.value = value;
    }
    
    // public void UI_Stage(EStageType stage)
    // {
    //     Stage.text = stage.ToString();
    // }
    //
    // public void UI_Phase(EPhaseType phase)
    // {
    //     Phase.text = phase.ToString();
    // }
    //
    //
    
    public void UI_TimerRefresh(float time)
    {
        Timer.text = $"{(int)time}";
    }
    
}
