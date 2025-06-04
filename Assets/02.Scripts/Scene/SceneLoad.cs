using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    [Header("참조")]
    public WorldManager worldManager;
    public Slider loadingBar;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI statusText;
    [Header("설정")]
    public string sceneName = "JeonTeaJun";

    public float cameraYOffset = 5f;
    public float cameraMoveDuration = 1f;

    [Header("로딩 BGM")]
    public AudioSource bgmSource; // AudioSource 컴포넌트 참조
    void Start()
    {
        if (bgmSource != null)
            bgmSource.Play();

        StartCoroutine(FullLoadRoutine());
    }

    private IEnumerator MoveCameraDown()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
            yield break;

        Vector3 startPos = mainCam.transform.position;
        Vector3 endPos = startPos - new Vector3(0, cameraYOffset, 0);

        float elapsed = 0f;
        while (elapsed < cameraMoveDuration)
        {
            float t = elapsed / cameraMoveDuration;
            mainCam.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // 최종 위치 보정
        mainCam.transform.position = endPos;
    }
    IEnumerator FullLoadRoutine()
    {
        // 가중치: 풀 20%, 월드 70%, 씬 10%
        const float wPool = 0.2f, wMap = 0.7f, wScene = 0.1f;

        // 1) 월드 생성
        statusText.text = "월드 생성 중...";
        yield return StartCoroutine(worldManager.InitWorldAsync(p =>
        {
            float overall =  p * wMap;
            loadingBar.value = overall;
            progressText.text = $"{Mathf.RoundToInt(overall * 100f)}%";
        }));
        yield return new WaitForSeconds(0.2f);

        // 2) 풀 초기화
        statusText.text = "풀 초기화 중...";
        loadingBar.value = 0;
        progressText.text = "0%";
        yield return StartCoroutine(ObjectPool.Instance.InitPoolAllAsync(p =>
        {
            float overall = p * wPool + wMap;
            loadingBar.value = overall;
            progressText.text = $"{Mathf.RoundToInt(overall * 100f)}%";
        }));
        yield return null;


        // 4) 카메라 이동
        yield return MoveCameraDown();

        // 3) 씬 비동기 Additive 로딩
        statusText.text = "게임 세상속으로 접속중~";
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            float overall = wPool + wMap + Mathf.Clamp01(op.progress / 0.9f) * wScene;
            loadingBar.value = overall;
            progressText.text = $"{Mathf.RoundToInt(overall * 100f)}%";
            yield return null;
        }

        loadingBar.value = 1f;
        progressText.text = "100%";
        yield return new WaitForSeconds(0.5f);
        // 로딩 UI 제거
        if (loadingBar != null) loadingBar.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
        if (statusText != null) statusText.gameObject.SetActive(false);


        if (bgmSource != null)
            bgmSource.Stop();

        // 5) 게임 씬 활성화
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        // 6) 로딩 씬 카메라 제거
        RemoveLoadingSceneCamera();

        // 7) 로딩 씬 언로드
        Scene currentScene = SceneManager.GetSceneByName("LoadingScene"); // 현재 씬 이름 정확히 적기
        if (currentScene.IsValid())
            yield return SceneManager.UnloadSceneAsync(currentScene);

        // 8) 후처리 이벤트 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void RemoveLoadingSceneCamera()
    {
        GameObject loadingCam = GameObject.FindWithTag("LoadingCamera");
        if (loadingCam != null)
            Destroy(loadingCam);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "JeonTeaJun")
        {
            worldManager.RegisterStageEvents(); // 이벤트 등록
            SceneManager.sceneLoaded -= OnSceneLoaded; // 한 번만 실행되게 제거
        }
    }

}
