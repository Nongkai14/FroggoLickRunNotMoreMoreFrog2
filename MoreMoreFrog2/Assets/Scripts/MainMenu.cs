using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;              // สำหรับ TMP_Text
using UnityEngine.UI;     // สำหรับ Slider
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject loadingPanel;   // Panel สำหรับ Loading
    public TMP_Text loadingText;      // TextMeshPro Text
    public Slider loadingBar;         // Slider สำหรับ Progress Bar

    void Start()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    public void PlayGame()
    {
        StartCoroutine(LoadSceneAsync("cutscene1")); // เปลี่ยนชื่อ scene ตามต้องการ
    }

    public void OpenShop()
    {
        StartCoroutine(LoadSceneAsync("ShopScene"));
    }

    public void OpenSettings()
    {
        StartCoroutine(LoadSceneAsync("SettingsScene"));
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
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

        // โหลดเสร็จแล้ว 90% → 100%
        loadingBar.value = 1f;
        loadingText.text = "Loading... 100%";
        yield return new WaitForSeconds(0.5f); // ดีเลย์นิดหน่อยให้ผู้เล่นเห็น 100%
        operation.allowSceneActivation = true;
    }
}
