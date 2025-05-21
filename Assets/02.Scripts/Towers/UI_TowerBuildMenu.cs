using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerBuildMenu : UI_Popup
{
    public List<Button> buttons;
    public List<BuildingType> buildingTypes;

    private void Awake()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            //람다 캡처(lambda capture) 문제
            int index = i;

            buttons[i].onClick.AddListener(() => OnBuildingSelected(index));
        }
    }

    private void OnBuildingSelected(int index)
    {
        TowerPlacer.Instance.SetSelectedBuilding(buildingTypes[index]);
        PlayerModeManager.Instance.SetMode(EPlayerMode.Build);
        Close();
    }
}
