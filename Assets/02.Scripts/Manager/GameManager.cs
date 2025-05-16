using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Run,
        Pause,
        GameOver
    }
    public GameState CurrentState = GameState.Run;
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
    
    // 게임 시작 => 설치하는 순간 게임 시작? 
    public void StartGame()
    {
        
    }
    // 캐릭터 사망시 GAMEOVER
    public void GameOver()
    {
        CurrentState = GameState.GameOver;
    }
    // 게임 pause 
    public void PauseGame()
    {
        CurrentState = GameState.Pause;
        Time.timeScale = 0;
    }
    // 게임 재시작 = 로딩 씬으로 이동
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    
}
