using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    [Header("����")]
    public WorldManager worldManager;
    public Slider loadingBar;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI statusText;
    [Header("����")]
    public string sceneName = "JeonTeaJun";

    public float cameraYOffset = 5f;
    public float cameraMoveDuration = 1f;

    void Start()
    {
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
        // ���� ��ġ ����
        mainCam.transform.position = endPos;
    }
    IEnumerator FullLoadRoutine()
    {
        // ����ġ: Ǯ 20%, ���� 70%, �� 10%
        const float wPool = 0.2f, wMap = 0.7f, wScene = 0.1f;

        // 1) Ǯ �ʱ�ȭ
        statusText.text = "Ǯ �ʱ�ȭ ��...";
        loadingBar.value = 0;
        progressText.text = "0%";
        yield return StartCoroutine(ObjectPool.Instance.InitPoolAllAsync(p =>
        {
            float overall = p * wPool;
            loadingBar.value = overall;
            progressText.text = $"{Mathf.RoundToInt(overall * 100f)}%";
        }));
        yield return null;

        // 2) ���� ����
        statusText.text = "���� ���� ��...";
        yield return StartCoroutine(worldManager.InitWorldAsync(p =>
        {
            float overall = wPool + p * wMap;
            loadingBar.value = overall;
            progressText.text = $"{Mathf.RoundToInt(overall * 100f)}%";
        }));
        yield return new WaitForSeconds(0.2f);


        // 3) �� �񵿱� �ε�
        statusText.text = "���� ��������� ������~";
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            float overall = wPool + wMap + op.progress * wScene;
            loadingBar.value = overall;
            progressText.text = $"{Mathf.RoundToInt(overall * 100f)}%";
            yield return null;
        }
        loadingBar.value = 1f;
        progressText.text = "100%";
        yield return new WaitForSeconds(0.5f);
        // �� ī�޶� ���� �ε巴�� �Ʒ��� �̵���Ű��
        yield return MoveCameraDown();

        SceneManager.sceneLoaded += OnSceneLoaded;
        op.allowSceneActivation = true;


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
            worldManager.RegisterStageEvents(); // �̺�Ʈ ���
            SceneManager.sceneLoaded -= OnSceneLoaded; // �� ���� ����ǰ� ����
        }
    }

}
