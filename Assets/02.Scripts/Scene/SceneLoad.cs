using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    public WorldManager worldManager;
    public Slider loadingBar;
    public TextMeshProUGUI progressText;

    void Start()
    {
        StartCoroutine(InitializeAndLoadGameScene());
    }

    IEnumerator InitializeAndLoadGameScene()
    {
        float progress = 0f;
        loadingBar.value = 0;
        progressText.text = "0%";

        yield return worldManager.InitWorldAsync(p =>
        {
            progress = p;
            loadingBar.value = progress;
            progressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        });

        yield return new WaitForSeconds(0.5f);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("JeonTeaJun");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "JeonTeaJun")
        {
            worldManager.RegisterStageEvents();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
