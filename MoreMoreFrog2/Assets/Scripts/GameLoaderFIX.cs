using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static GameLoader Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ให้คงอยู่ข้าม Scene
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool needResetOnLoad = false;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (needResetOnLoad)
        {
            NewPlayerController player = FindFirstObjectByType<NewPlayerController>();
            if (player != null)
            {
                player.ResetPlayerState();
            }

            needResetOnLoad = false;
        }
    }
}
