using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.Timeline;

public class UIManager : MonoBehaviour  
{
    public static UIManager instance;
    [Header("슬라이더")]
    public Slider CurrentTime;
    public Slider HpBar;
    public Slider Mpbar;
    public Slider EXPBar;
    [Header("텍스트")]
    public TextMeshProUGUI[] Currencies;
    public TextMeshProUGUI CurrentEnemy;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI Tutorial;
    public TextMeshProUGUI TutorialCount;
    [Header("오브젝트")]
    public Image[] CurrenciesObjects;
    public GameObject GameOverPanel;
    public GameObject TimerObject;
    public GameObject CountObject;
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
        CurrentEnemy.text = $"{EnemyManager.Instance.ActiveEnemies.Count}";
    }
    // 재화 변경
    public void RefreshCurrency()
    {
        Currencies[0].text = $"{CurrencyManager.instance.CurrentCurrency().Gold}";
        Currencies[1].text = $"{CurrencyManager.instance.CurrentCurrency().Wood}";
        Currencies[2].text = $"{CurrencyManager.instance.CurrentCurrency().Stone}";
    }
    // 게임오버
    public void UI_GameOver()
    {
      GameOverPanel.SetActive(true);
    }

    public void UI_ObjectOnOff(GameObject ui)
    {
        TimerObject.SetActive(false);
        CountObject.SetActive(false);
        
        ui.SetActive(true);
    }
    // tab할 때 재황 표시하기
    public void UI_AppearCurrency()
    {
        foreach (Image cur in CurrenciesObjects)
        {
            cur.DOKill();
        }
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(CurrenciesObjects[0].rectTransform.DOAnchorPosX( -30f, 0.1f).SetEase(Ease.OutCirc));
        sequence.Join(CurrenciesObjects[1].rectTransform.DOAnchorPosX(-30f, 0.2f).SetEase(Ease.OutCirc));
        sequence.Join(CurrenciesObjects[2].rectTransform.DOAnchorPosX(-30f, 0.3f).SetEase(Ease.OutCirc));
        
    }

    public void UI_DisappearCurrency()
    {   
        foreach (Image cur in CurrenciesObjects)
        {
            cur.DOKill();
        }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(CurrenciesObjects[0].rectTransform.DOAnchorPosX( 250f, 0.1f).SetEase(Ease.OutCirc));
        sequence.Join(CurrenciesObjects[1].rectTransform.DOAnchorPosX(250f, 0.2f).SetEase(Ease.OutCirc));
        sequence.Join(CurrenciesObjects[2].rectTransform.DOAnchorPosX(250f, 0.3f).SetEase(Ease.OutCirc));
    }
    //튜토리얼 전체 변경
    public void UI_TutorialRefresh(string tutorial,float current, int require )
    {
        Tutorial.text = $"{tutorial}";
        TutorialCount.text = $"{current} / {require}";
    }

    public void UI_TutoCurrentCountRefresh(float current, int require)
    {
        TutorialCount.text = $"{(int)current} / {require}";
    }
    // 튜토리얼 끝 튜토리얼 확인 창 사라지게 하기
    public void UI_TutorialEnd(float readyTime, float timer)
    {
        UI_SetMaxTimer(readyTime);
        TutorialCount.gameObject.SetActive(false);
        Tutorial.gameObject.SetActive(false);
    }
    // hp, mp 한번에 세팅
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
    public void UI_TimerSlider(float value) => RefreshSlider(CurrentTime, value);
    public void UI_MpSlider(float value)=> RefreshSlider(Mpbar, value);
    public void UI_HpSlider(float value) => RefreshSlider(HpBar, value);
    //--------------------------------------------------------------------
    private void SetSliderValue(Slider slider, float value)
    {
        slider.maxValue = value;
        SlowRefreshSlider(slider, value);
    }
    //슬라이더를 조절하는 함수 베이스
    private void RefreshSlider(Slider slider, float value)
    {
        slider.value = value;
    }
    public void UI_TimerRefresh(float time)
    {
        Timer.text = $"{(int)time}";
        UI_TimerSlider(time);
    }

    public void EnemyCountSlider(float value, float duration)
    {
        CurrentTime.maxValue = value;
        EnemyRefreshSlider(value, duration);
    }
    
    private void SlowRefreshSlider(Slider slider, float value)
    {
        // 현재 트윈이 있다면 종료
        slider.DOKill();
    
        // 목표 값으로 부드럽게 이동 (0.3초 정도 추천)
        slider.DOValue(value, 3).SetEase(Ease.OutQuad);
    }
    public void EnemyRefreshSlider(float value, float duration)
    {
        // 현재 트윈이 있다면 종료
        CurrentTime.DOKill();
    
        // 목표 값으로 부드럽게 이동 (0.3초 정도 추천)
        CurrentTime.DOValue(value, duration).SetEase(Ease.OutQuad);
    }
}
