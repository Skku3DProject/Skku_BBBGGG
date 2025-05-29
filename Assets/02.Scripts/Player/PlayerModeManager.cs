using System;
using UnityEngine;
public enum EPlayerMode
{
    Weapon,
    Pickaxe,
    Build,
    Block
}
public class PlayerModeManager : MonoBehaviour
{
    public static PlayerModeManager Instance { get; private set; }

    public EPlayerMode CurrentMode { get; private set; } = EPlayerMode.Weapon;
    public static event Action<EPlayerMode> OnModeChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
        //    SetMode(EPlayerMode.Weapon);
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //    SetMode(EPlayerMode.Pickaxe);
        //else if(Input.GetKeyDown(KeyCode.Alpha5))
        //    SetMode(EPlayerMode.Block);
    }

    public void SetMode(EPlayerMode mode)
    {
        if (CurrentMode == mode) return;

        CurrentMode = mode;
        OnModeChanged?.Invoke(mode);


    }
}
