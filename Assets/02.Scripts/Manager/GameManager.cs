using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Ready,
        Run,
        Pause,
        GameOver
    }
    
    public GameState CurrentState = GameState.Ready;
    
    // 싱글톤
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // 웨이브 타이머
    private float _timer = 0;

    private void Update()
    {
        switch (CurrentState)
        {
            case GameState.Run:
            {
                StartGame();
                break;
            }
            case GameState.Pause:
            {
                PauseGame();
                break;
            }
            case GameState.GameOver:
            {
                GameOver();
                break;
            }
        }
    }
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }
    // 게임 시작 => 설치하는 순간 게임 시작? 
    private void StartGame()
    {
        _timer += Time.deltaTime;
    }
    // 거점이 파괴되면 게임오버
    private void GameOver()
    {
        CurrentState = GameState.GameOver;
        UIManager.instance.UI_GameOver();
    }
    // 게임 pause 
    private void PauseGame()
    {
        CurrentState = GameState.Pause;
        Time.timeScale = 0;
    }
    // 게임 재시작 = 로딩 씬으로 이동 -> 버튼으로 구현 예정
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    
}
