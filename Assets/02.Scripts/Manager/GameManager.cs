using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 싱글톤
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // 게임 시작
    public void StartGame()
    {
        
    }
    // 게임 종료
    public void EndGame()
    {
        
    }
    // 게임 pause
    public void PauseGame()
    {
        
    }
    // 게임 재시작
    public void RestartGame()
    {
        
    }
}
