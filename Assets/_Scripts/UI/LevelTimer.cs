using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTimer : MonoBehaviour
{
    [Header("ตั้งค่าการใช้งาน (ติ๊กออกถ้าไม่ต้องการใช้เวลา)")]
    public bool useTimer = true;
    public GameObject timerUIObject; // ลาก Object ตัวแม่ที่ชื่อ "Time" มาใส่ เพื่อซ่อน/โชว์ทั้งชุด

    [Header("ตั้งค่าเวลา")]
    public float initialTimeLimit = 150f; // 2.30 นาที = 150 วินาที
    private float currentTime;

    [Header("UI")]
    public Text timeText; // ลาก Text (Legacy) ของคุณมาใส่

    private bool isTimeUp = false;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // ถ้าเราตั้งค่าไม่ใช้งาน ให้ซ่อน UI เวลาทิ้งไปเลย และไม่ต้องนับเวลา
        if (!useTimer)
        {
            if (timerUIObject != null) timerUIObject.SetActive(false);
            return;
        }

        // ถ้าใช้งาน ให้โชว์ UI
        if (timerUIObject != null) timerUIObject.SetActive(true);

        // เช็คว่ากลับมาจากมินิเกมหรือเปล่า?
        if (GameManager.Instance != null && GameManager.Instance.isReturningFromMiniGame)
        {
            currentTime = GameManager.Instance.savedTime; // ดึงเวลาเก่ามาใช้ต่อ
        }
        else
        {
            currentTime = initialTimeLimit; // เริ่มด่านใหม่ ใช้เวลาเต็ม
        }

        UpdateTimerUI();
    }

    private void Update()
    {
        // ถ้ายกเลิกการใช้ หรือ เวลาหมดแล้ว ให้หยุดทำงาน
        if (!useTimer || isTimeUp) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimeUp = true;
            TimeRanOut(); // เรียกฟังก์ชันตายเมื่อเวลาหมด
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60F);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
            // แสดงผลรูปแบบ 00.00 ตามรูป UI ของคุณ
            timeText.text = string.Format("{0:00}.{1:00}", minutes, seconds);
        }
    }

    private void TimeRanOut()
    {
        Debug.Log("หมดเวลา! กบเขียวขิต!");
        if (player != null)
        {
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            player.GetComponent<Animator>().SetTrigger("death");

            // รอแอนิเมชันตายเล่น 0.8 วิ แล้วเรียกหน้าต่าง Game Over
            Invoke("TriggerGameOver", 0.8f);
        }
    }

    private void TriggerGameOver()
    {
        // สั่งให้ระบบค้นหาสคริปต์ GameOverManager แล้วแสดงหน้าต่างขึ้นมา
        GameOverManager gameOverSystem = Object.FindFirstObjectByType<GameOverManager>();
        if (gameOverSystem != null)
        {
            gameOverSystem.ShowGameOver();
        }
    }

    private void RestartLevel()
    {
        if (GameManager.Instance != null) GameManager.Instance.ResetStateOnDeath();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ฟังก์ชันนี้ให้ QuestionPoint มาดึงเวลาไปเก็บ
    public float GetCurrentTime()
    {
        return currentTime;
    }
}