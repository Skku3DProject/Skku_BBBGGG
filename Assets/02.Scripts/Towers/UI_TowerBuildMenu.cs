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
            //���� ĸó(lambda capture) ����
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
