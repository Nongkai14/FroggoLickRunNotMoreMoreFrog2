using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject loadingPanel;
    public TMPro.TMP_Text loadingText;
    public UnityEngine.UI.Slider loadingBar;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        PlayerPrefs.DeleteKey("Scene1_TutorialShown");
        StartCoroutine(LoadSceneAsync("Scene1")); // เปลี่ยนชื่อ Scene ได้ตามต้องการ
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            loadingText.text = $"Loading... {progress * 100:F0}%";
            yield return null;
        }

        loadingBar.value = 1f;
        loadingText.text = "Loading... 100%";
        yield return new WaitForSeconds(0.5f);

        operation.allowSceneActivation = true;
    }

    public void SkipCutscene()
    {
        videoPlayer.Stop();
        OnVideoEnd(videoPlayer); // โหลดทันทีเหมือนจบ
    }
}
