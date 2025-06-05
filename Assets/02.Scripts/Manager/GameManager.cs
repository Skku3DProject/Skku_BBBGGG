using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Run,
    Pause,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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

    //현재 게임 상태 변경하기
    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }
        
        CurrentState = newState;
        
        switch (CurrentState)
        {
            case GameState.Run:
            {
                ContinueGame();
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
    // 게임 시작 => 설치하는 순간 게임 시작? 
    private void StartGame()
    {
        Time.timeScale = 1;
    }
    // 거점이 파괴되면 게임오버
    private void GameOver()
    {
        UIManager.instance.UI_GameOver();

        StartCoroutine(GameOver_Coroutine());

    }
    // 게임 pause 
    private void PauseGame()
    {   
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }
    // 게임 계속하기
    public void ContinueGame()
    {
        CurrentState = GameState.Run;
        Time.timeScale = 1;
        PopUpManager.Instance.PauseBackground.gameObject.SetActive(false);
    }
    // 게임 재시작 = 로딩 씬으로 이동 -> 버튼으로 구현 예정
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public IEnumerator GameOver_Coroutine()
    {
        yield return new WaitForSeconds(5f);
        
        SceneManager.LoadScene("TitleScene");
    }
    
}
