using DG.Tweening;
using System.Collections;
using TMPro;
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

    [Header("카메라 참조")]
    public Camera loadingSceneCamera;

    public Image Dog;
    void Start()
    {
        if (bgmSource != null)
            bgmSource.Play();

        StartCoroutine(FullLoadRoutine());

        // 띠용띠용 커졌다 작아졌다 반복
        Dog.rectTransform
            .DOScale(new Vector3(1.2f, 1.2f, 1f), 0.5f) // 1.2배 커짐
            .SetLoops(-1, LoopType.Yoyo)               // 반복, 원래 크기로 복귀
            .SetEase(Ease.InOutSine);                  // 부드러운 애니메이션

    }

    private void Update()
    {
    }

    IEnumerator FullLoadRoutine()
    {
        // 가중치: 풀 20%, 월드 70%, 씬 10%
        const float wPool = 0.2f, wMap = 0.7f, wScene = 0.1f;

        // 1) 월드 생성
        statusText.text = "잔디나 버섯을 부수면 스태미너를 회복할 수 있습니다.";
        bool changedText = false;

        yield return StartCoroutine(worldManager.InitWorldAsync(p =>
        {
            float overall = p * wMap;
            loadingBar.value = overall;
            progressText.text = $"{Mathf.RoundToInt(overall * 100f)}%";

            // 절반 넘었을 때 텍스트 변경 (1회만 실행되도록)
            if (!changedText && p >= 0.5f)
            {
                statusText.text = "맵 전역에 보물상자가 숨겨져 있습니다.";
                changedText = true;
            }
        }));
        yield return new WaitForSeconds(0.2f);

        // 2) 풀 초기화
        statusText.text = "게임 시작 준비가 거의 끝나갑니다...";
        loadingBar.value = 0;
        progressText.text = "0%";
        yield return StartCoroutine(ObjectPool.Instance.InitPoolAllAsync(p =>
        {
            float overall = p * wPool + wMap;
            loadingBar.value = overall;
            progressText.text = $"{Mathf.RoundToInt(overall * 100f)}%";
        }));
        yield return null;

        // 3) 씬 비동기 Additive 로딩
        statusText.text = "게임 스타또~!";
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
            worldManager.RegisterStageEventsForSceneLoad(); // 이벤트 등록
            SceneManager.sceneLoaded -= OnSceneLoaded; // 한 번만 실행되게 제거
        }
    }

}
