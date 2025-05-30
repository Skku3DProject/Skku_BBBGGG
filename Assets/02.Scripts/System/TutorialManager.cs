using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum TutorialType
{
    Moving,
    SwordEquipment,
    SwordAttack,
    BowEquipment,
    BowAttack,
    MagicEquipment,
    MagicAttack,
    PicAxeEquipment,
    CollectWood,
    CollectRock,
    BlockEquipment,
    CreateBlock,
    CreateTower,
    InputKeyCodeK,
    Tab,
    
    count
}

public class TutorialManager : MonoBehaviour
{
    public Action<TutorialType, float> OnProgress;
    public List<SO_Tutorial> tutorialDataList;
    [SerializeField] private Queue<TutorialBase> tutorialQueue = new();
    private TutorialBase currentStep;
    public TutorialBase CurrentTutorial => currentStep;
    public static TutorialManager Instance { get; private set; }
    public float CurrentCount = 0;

    private void Awake()
    {
        Instance = this;

        foreach (var data in tutorialDataList)
        {
            tutorialQueue.Enqueue(CreateStepFromData(data));
        }

    }

    private void Start()
    {
        TutorialEvent.OnProgress += AddProgress;
        
        LoadNextStep();
    }

    private TutorialBase CreateStepFromData(SO_Tutorial data)
    {
        return new CollectTutorial(data);
    }

    public void AddProgress(TutorialType type, float amount)
    {
        if (currentStep == null)
        {
            return;
        }

        currentStep.OnProgress(type, amount);
        CurrentCount += amount;
        UIManager.instance.UI_TutoCurrentCountRefresh(CurrentCount, currentStep.RequiredAmount);
        if (currentStep.IsCompleted)
        {
            currentStep.OnComplete();

            CurrencyManager.instance.Add(currentStep.currency);
            CurrentCount = 0;
            LoadNextStep();
        }
    }

    private void LoadNextStep()
    {
        if (tutorialQueue.Count > 0)
        {
            currentStep = tutorialQueue.Dequeue();
            currentStep.Start();
            UIManager.instance.UI_TutorialRefresh(currentStep.MainTitle, currentStep.SubTitle, CurrentCount, currentStep.RequiredAmount);
        }
        else
        {
            StageManager.instance.TutorialEnd();
            gameObject.SetActive(false);
        }
    }

    public void TutorialSkip()
    {
        tutorialQueue.Clear();
        LoadNextStep();
    }

    private void OnDisable()
    {
        TutorialEvent.OnProgress -= AddProgress;
    }
}