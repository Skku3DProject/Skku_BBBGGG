using System;
using Unity.VisualScripting;
using UnityEngine;

public enum EPhaseType
{
    None,
    Tutorial,
    Ready,
    Combat,
    End
    
}
public enum EStageType
{
    Tutorial,
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    Stage6,
    Stage7,
    Stage8,
    Stage9,
    
    Count
}

public class StageManager : MonoBehaviour
{
    public static StageManager instance;
    private float _resetTime = 2;
    private float _reducedTime = 0.3f;
    private float _settingTime = 2;
    private float _timer = 30f;
    [SerializeField] private float _readyTime = 1f;
    
    public Action OnCombatStart;
    public Action OnCombatEnd;
    
    [SerializeField]private EStageType _currentStage = EStageType.Tutorial;
    [SerializeField]private EPhaseType _currentPhase = EPhaseType.Tutorial;
    public EPhaseType CurrentPhase => _currentPhase;
    private float _enemyCount => EnemyManager.Instance.ActiveEnemies.Count;
    // 에너미 카운트 세기
    private bool _allEnemiesDead => EnemyManager.Instance.ActiveEnemies.Count <= 0;  
    
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
    private void Update()
    {
        TutorialSkip();
        
        switch (_currentPhase)
        {
            case EPhaseType.None:
                _settingTime -= Time.deltaTime;
                if (_settingTime <= 0)
                {
                    SliderSetting();
                }
                break;
            case EPhaseType.Ready:
                _timer -= Time.deltaTime;
                UIManager.instance.UI_TimerRefresh(_timer);
                if (_timer <= 0)
                {
                    CombatStart();
                }
                break;
            
            case EPhaseType.Combat:
                UIManager.instance.CurrentCountRefresh();
                UIManager.instance.EnemyRefreshSlider(_enemyCount, _reducedTime);
                if (_allEnemiesDead) 
                {
                    CombatEnd();
                }
                break;
            
        }
    }

    public void TutorialSkip()
    {
        if (Input.GetKeyDown(KeyCode.F1) && _currentPhase == EPhaseType.Tutorial)
        {
            TutorialEnd();
            TutorialManager.Instance.TutorialSkip();
        }
        // else if (Input.GetKeyDown(KeyCode.J) && _currentPhase != EPhaseType.Tutorial)
        // {
        //     CombatStart();
        // }
    }
    public void TutorialEnd()
    {
        _currentPhase = EPhaseType.None;
        UIManager.instance.UI_TutorialEnd();
    }

    private void SliderSetting()
    {
        _currentPhase = EPhaseType.Ready;
        
        UIManager.instance.UI_StageStartMention(1);
        UIManager.instance.UI_ObjectOnOff(UIManager.instance.TimerObject);   
    }
    public void CombatEnd()
    {
        _currentPhase = EPhaseType.End;
        _timer = _readyTime;
        _settingTime = _resetTime;
        UIManager.instance.UI_SetMaxTimer(_readyTime);
        
        OnCombatEnd?.Invoke();
        
        NextStage();
    }

    public void CombatStart()
    {
        _currentPhase = EPhaseType.Combat;
        UIManager.instance.UI_StageStartMention(0);
        UIManager.instance.UI_ObjectOnOff(UIManager.instance.CountObject);
        
        OnCombatStart?.Invoke();
        
        UIManager.instance.EnemyCountSlider(_enemyCount, _resetTime);
    }
    // 다음 스테이지 전환
    private void NextStage()
    {
        int nextIndex = (int)_currentStage + 1;
        if (nextIndex < (int)EStageType.Count)
        {
            _currentStage = (EStageType)nextIndex;
            _currentPhase = EPhaseType.None;
        }
        else
        {
            Debug.Log("스테이지 끝");
        }
        
    }
    
    public EStageType GetCurrentStage()
    {
        return _currentStage;
    }
}
