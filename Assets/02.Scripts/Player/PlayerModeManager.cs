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
    public void SetMode(EPlayerMode mode)
    {
        if (CurrentMode == mode) return;

        CurrentMode = mode;
        OnModeChanged?.Invoke(mode);


    }
}
