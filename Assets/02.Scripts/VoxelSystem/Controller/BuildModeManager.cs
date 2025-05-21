//using UnityEngine;

//public class BuildModeManager : MonoBehaviour
//{
//    public static BuildModeManager Instance { get; private set; }
//    public bool IsBuildMode { get; private set; } = false;

//    void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//        }
//        else
//        {
//            Instance = this;
//        }
//    }

//    public void ToggleBuildMode()
//    {
//        IsBuildMode = !IsBuildMode;
//        Debug.Log("Build Mode: " + (IsBuildMode ? "ON" : "OFF"));
//    }

//    public void SetBuildMode(bool on)
//    {
//        IsBuildMode = on;
//    }
//}
