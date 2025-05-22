using System;
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
    
    private float _timer;
    [SerializeField] private float _readyTime = 1f;
    [SerializeField] private float _combatTime = 1f;

    public Action OnStageStart;
    public Action OnStageEnd;
    public Action OnCombatStart;
    public Action OnCombatEnd;
    
    private EStageType _currentStage = EStageType.Tutorial;
    private EPhaseType _currentPhase = EPhaseType.None;
    // 몬스터 카운트
    public int EnemyCount;
    private bool _allEnemiesDead = false;
    
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

    private void Start()
    {
        TutorialEnd();
    }

    private void Update()
    {
        switch (_currentPhase)
        {
            case EPhaseType.Ready:
                _timer -= Time.deltaTime;
                UIManager.instance.UI_TimerRefresh(_timer);
                if (_timer <= 0)
                {
                    CombatStart();
                }

                break;
            
            case EPhaseType.Combat:
                _timer -= Time.deltaTime;
                UIManager.instance.UI_TimerRefresh(_timer);
                
                if (_timer <= 0 || _allEnemiesDead)
                {
                    CombatEnd();
                }
                break;
            
        }
    }

    public void TutorialEnd()
    {
        _currentStage = EStageType.Stage1;
        _currentPhase = EPhaseType.Ready;
        _timer = _readyTime;
        // UIManager.instance.UI_Stage(_currentStage);
        UIManager.instance.UI_SetMaxTimer(_readyTime);
        UIManager.instance.UI_TimerRefresh(_timer);
    }

    private void StartReadyPhase()
    {
        _currentPhase = EPhaseType.Ready;
        _timer = 0;
    }
    public void CombatEnd()
    {
        _currentPhase = EPhaseType.End;
        _timer = _readyTime;
        
        UIManager.instance.UI_SetMaxTimer(_readyTime);
        OnCombatEnd?.Invoke();
        
        NextStage();
    }

    private void CombatStart()
    {
        _currentPhase = EPhaseType.Combat;
        _timer = _combatTime;
        
        UIManager.instance.UI_SetMaxTimer(_combatTime);
        OnCombatStart?.Invoke();
        _allEnemiesDead = false;
    }
    // 다음 스테이지 전환
    private void NextStage()
    {
        int nextIndex = (int)_currentStage + 1;
        if (nextIndex < (int)EStageType.Count)
        {
            _currentStage = (EStageType)nextIndex;
            _currentPhase = EPhaseType.Ready;
        }
        else
        {
            Debug.Log("스테이지 끝");
        }
        UIManager.instance.UI_TimerRefresh(_timer);
        // UIManager.instance.UI_Stage(_currentStage);
    }

    public void OnAllEnemiesDefeated()
    {
        _allEnemiesDead = true;
    }

    public EStageType GetCurrentStage()
    {
        return _currentStage;
    }
}
