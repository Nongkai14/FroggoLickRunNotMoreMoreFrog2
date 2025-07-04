using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    // เรียกจากปุ่ม "Play Again"
    public void PlayAgain()
    {
        Time.timeScale = 1f;

        if (GameLoader.Instance != null)
        {
            GameLoader.Instance.needResetOnLoad = true;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // เรียกจากปุ่ม "Main Menu"
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // แก้ชื่อเป็นชื่อ Scene เมนูของคุณจริง ๆ
    }

    // เรียกเมื่อ Game Over เกิดขึ้น เช่น HP <= 0
    public void ShowGameOverScreen(GameObject gameOverPanel)
    {
        Time.timeScale = 0f;  // หยุดเวลา
        gameOverPanel.SetActive(true);
    }
}
