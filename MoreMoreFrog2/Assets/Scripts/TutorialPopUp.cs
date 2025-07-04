using UnityEngine;

public class TutorialPopupAnyKey : MonoBehaviour
{
    public GameObject tutorialPanel;
    private bool tutorialActive = false;

    private const string tutorialKey = "Scene1_TutorialShown";

    void Start()
    {

        if (!PlayerPrefs.HasKey(tutorialKey))
        {
            tutorialPanel.SetActive(true);
            tutorialActive = true;

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (tutorialActive && Input.anyKeyDown)
        {
            CloseTutorial();
        }
    }

    void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        tutorialActive = false;

        Time.timeScale = 1f;
        PlayerPrefs.SetInt(tutorialKey, 1);
        PlayerPrefs.Save();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
