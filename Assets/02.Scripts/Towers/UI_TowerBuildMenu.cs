using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerBuildMenu : UI_Popup
{
    public List<Button> buttons;
    public List<BuildingType> buildingTypes;
    public static bool isBuildMode = false;

    private void Awake()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            //���� ĸó ����
            int index = i;

            buttons[i].onClick.AddListener(() => OnBuildingSelected(index));
        }
    }

    private void OnBuildingSelected(int index)
    {
        PlayerTowerPlacer.Instance.SetSelectedBuilding(buildingTypes[index]);
        PlayerModeManager.Instance.SetMode(EPlayerMode.Build);
        Close();
    }
}
