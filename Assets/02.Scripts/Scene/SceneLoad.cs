using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    public WorldManager worldManager;

    void Start()
    {
        StartCoroutine(InitializeAndLoadGameScene());
    }

    IEnumerator InitializeAndLoadGameScene()
    {
        yield return null;
        worldManager.InitWorld(); // 맵 생성

        yield return new WaitForSeconds(1f); // 생성 완료 후 잠깐 대기

        SceneManager.sceneLoaded += OnSceneLoaded; // 로드 콜백 등록
        SceneManager.LoadScene("JeonTeaJun");
    }

    // 씬 전환 후 호출될 콜백
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "JeonTeaJun")
        {
            worldManager.RegisterStageEvents(); // 이벤트 등록
            SceneManager.sceneLoaded -= OnSceneLoaded; // 한 번만 실행되게 제거
        }
    }
}
