using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("ลาก PausePanel ของคุณมาใส่ที่นี่")]
    public GameObject pausePanel;

    [Header("ตั้งชื่อซีนหน้าเมนูให้ตรงกับที่สร้างไว้")]
    public string menuSceneName = "Menu";

    private void Start()
    {
        // บังคับปิดหน้าต่าง Pause ไว้ก่อนตอนเริ่มเกมเสมอ
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true); // เปิดหน้าต่าง Pause
            Time.timeScale = 0f; // แช่แข็งเวลาในเกม!
        }
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // ปิดหน้าต่าง Pause
            Time.timeScale = 1f; // ปล่อยเวลาให้เดินตามปกติ
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // ⚠️ ต้องคืนค่าเวลาก่อน ไม่งั้นด่านใหม่จะค้าง

        if (GameManager.Instance != null)
        {
            // เรียกใช้ระบบล้างเซฟตอนตายจาก GameManager ของคุณ
            GameManager.Instance.ResetStateOnDeath();
        }

        // โหลดซีนปัจจุบันซ้ำอีกครั้ง
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- 🟢 ฟังก์ชันสำหรับปุ่ม "Menu" ออกกลางคัน ---
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // ⚠️ ต้องคืนค่าเวลาก่อนเช่นกัน

        if (GameManager.Instance != null)
        {
            // 🟢 เรียกฟังก์ชันล้างด่านให้สะอาดหมดจดตามที่คุณคิดเลยครับ!
            // มันจะเคลียร์หีบ มอนสเตอร์ เหรียญ เช็คพอยต์ ให้กลับมาเริ่มต้นใหม่
            GameManager.Instance.ResetStateOnDeath();
        }

        // วาร์ปไปซีนเมนูหลัก
        SceneManager.LoadScene(menuSceneName);
    }
}