using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameOverPanel;

    [Header("UI Texts (ลาก Text มาใส่)")]
    public Text timeText;
    public Text heartText;

    [Header("System References")]
    public HealthSystem playerHealth;
    public LevelTimer levelTimer;

    [Header("Settings")]
    public string menuSceneName = "Menu";

    private void Start()
    {
        // ซ่อนหน้าต่าง Game Over ไว้ก่อนตอนเริ่มเกม
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    // ฟังก์ชันนี้จะถูกเรียกตอนที่กบเขียวตายแบบของจริง (เลือดหมด หรือ เวลาหมด)
    public void ShowGameOver()
    {
        Time.timeScale = 0f; // หยุดเวลาในเกม
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // 1. ดึงค่าปัจจุบัน
        int currentHearts = playerHealth != null ? playerHealth.currentHealth : 0;
        float currentTime = (levelTimer != null && levelTimer.useTimer) ? levelTimer.GetCurrentTime() : 0f;
        int timeInt = Mathf.FloorToInt(currentTime);

        // 2. เช็คเงื่อนไขเวลา
        if (levelTimer != null && levelTimer.useTimer && timeInt <= 0)
        {
            timeText.text = "Time's Up! Game Over!";
            timeText.color = Color.red; // เปลี่ยนข้อความเป็นสีแดง
        }
        else
        {
            timeText.text = timeInt.ToString() + " seconds left";
            timeText.color = Color.black; // เปลี่ยนกลับเป็นสีปกติ (ปรับสีดำ/ขาวตามใจชอบได้เลยครับ)
        }

        // 3. เช็คเงื่อนไขหัวใจ
        if (currentHearts <= 0)
        {
            heartText.text = "No Heart Left!";
            heartText.color = Color.red;
        }
        else
        {
            // แก้ข้อความตรงบรรทัดนี้ครับ
            heartText.text = "You have " + currentHearts.ToString() + " lives left";
            heartText.color = Color.black;
        }
    }

    // --- ฟังก์ชันสำหรับปุ่ม Retry ---
    public void OnClickRetry()
    {
        Time.timeScale = 1f; // คืนค่าเวลาก่อน

        // ล้างข้อมูลเซฟในด่านทิ้ง (เพื่อให้มอนสเตอร์และไอเทมกลับมาเกิดใหม่)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetStateOnDeath();
        }

        // โหลดซีนปัจจุบันซ้ำอีกครั้ง
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- ฟังก์ชันสำหรับปุ่ม Menu ---
    public void OnClickMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isReturningFromMiniGame = false;
        }
        SceneManager.LoadScene(menuSceneName);
    }
}