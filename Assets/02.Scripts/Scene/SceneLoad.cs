using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    public Slider LoadingBar;
    public int NextSceneIndex;
    public TextMeshProUGUI ProgressText;

    private void Start()
    {
        StartCoroutine(SceneLoading_Coroutine());
    }
    
    // 씬 로드
    private IEnumerator SceneLoading_Coroutine()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(NextSceneIndex);
        while (ao.isDone == false)
        {
            LoadingBar.value = ao.progress;
            ProgressText.text = $"{ao.progress * 100}% ";
            yield return null;
        } 
    }
    
}
