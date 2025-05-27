using UnityEngine;

public class TutorialInput : MonoBehaviour
{
    private void Update()
    {
        if (StageManager.instance.GetCurrentStage() != EStageType.Tutorial)
        {
            return;
        }
        
        CheckInput();
    }

    private void CheckInput()
    {
        var currentTutorial = TutorialManager.Instance.CurrentTutorial;
       
        if (currentTutorial == null)
        {
            return;
        }
        
        switch (currentTutorial.Type)
        {
            case TutorilaType.Moving :
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.D))
                {
                    TutorialManager.Instance.AddProgress(TutorilaType.Moving, 1);
                    Debug.Log("무빙 입력");
                }
                break;
            case TutorilaType.SwordEquipment :
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    TutorialManager.Instance.AddProgress(TutorilaType.SwordEquipment, 1);
                }
                break;
            case TutorilaType.SwordAttack :
                if (Input.GetMouseButtonDown(0))
                {
                    TutorialManager.Instance.AddProgress(TutorilaType.SwordAttack, 1);
                }
                break;
            
        }
    }
}
