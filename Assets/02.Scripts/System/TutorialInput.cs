using UnityEngine;

public class TutorialInput : MonoBehaviour
{
    private float _moveTime = 0.01f;
    private EEquipmentType _currentEquipmentType => PlayerEquipmentManager.Instance.GetCurrentEquipType();
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
            case TutorialType.Moving :
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.D))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.Moving, _moveTime);
                }
                break;
            case TutorialType.SwordEquipment :
                if (Input.GetKeyDown(KeyCode.Alpha1) && _currentEquipmentType == EEquipmentType.Sword)
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.SwordEquipment, 1);
                }
                break;
            case TutorialType.SwordAttack :
                if (Input.GetMouseButtonDown(0))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.SwordAttack, 1);
                }
                break;
            case TutorialType.BowEquipment :
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.BowEquipment, 1);
                }
                break;
            case TutorialType.BowAttack:
                if (Input.GetMouseButtonDown(1) && _currentEquipmentType == EEquipmentType.Bow)
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.BowAttack, 1);
                }
                break;
            case TutorialType.MagicEquipment :
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.MagicEquipment, 1);
                }
                break;
            case TutorialType.MagicAttack :
                if (Input.GetMouseButtonDown(0) && _currentEquipmentType == EEquipmentType.Magic)
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.MagicAttack, 1);
                }
                break;
            case TutorialType.PicAxeEquipment:
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.PicAxeEquipment, 1);
                }
                break;
            case TutorialType.BlockEquipment:
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.BlockEquipment, 1);
                }
                break;
            case TutorialType.Tab :
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.Tab, 1);
                }
                break;
            case TutorialType.CreateBlock:
                if (Input.GetMouseButtonDown(0))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.CreateBlock, 1);
                }
                break;
            case TutorialType.InputKeyCodeK:
                if (Input.GetKeyDown(KeyCode.K))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.InputKeyCodeK, 1);
                }
                break;
            case TutorialType.Return :
                if (Input.GetKeyDown(KeyCode.R))
                {
                    TutorialEvent.OnProgress?.Invoke(TutorialType.Return, 1);
                }
                break;
        }
    }
}
