using System;
using UnityEngine;
using System.Collections.Generic;

public enum TutorilaType
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
    
    count
}
public class TutorialManager : MonoBehaviour
{
    public List<SO_Tutorial> tutorialDataList;

    private Queue<TutorialBase> tutorialQueue = new();
    private TutorialBase currentStep;
    public TutorialBase CurrentTutorial => currentStep;
    public static TutorialManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        foreach (var data in tutorialDataList)
        {
            tutorialQueue.Enqueue(CreateStepFromData(data));
        }

        LoadNextStep();
    }

    private TutorialBase CreateStepFromData(SO_Tutorial data)
    {
        return data.TutoType switch
        {
            TutorilaType.Moving => new SecondTutorial(data),
            TutorilaType.CollectWood => new FirstTutorial(data),
            
            // 다른 타입 추가 가능
            _ => throw new NotImplementedException($"Unsupported tutorial type: {data.TutoType}")
        };
    }

    public void AddProgress(TutorilaType type, int amount)
    {
        if (currentStep == null) return;

        currentStep.OnProgress(type, amount);
        Debug.Log("하나 먹음");
        if (currentStep.IsCompleted)
        {
            currentStep.OnComplete();
            LoadNextStep();
        }
    }

    private void LoadNextStep()
     {
         if (tutorialQueue.Count > 0)
         {
             currentStep = tutorialQueue.Dequeue();
             currentStep.Start();
         }
         else
         {
             Debug.Log("튜토리얼 완료!");
         }
     }
     
 }
