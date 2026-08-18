using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // คืนเวลาเสมอ
        // สั่งเคลียร์ของถ้าจำเป็น
        SceneManager.LoadScene("Menu");
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelName);
    }

    // ฯลฯ
}