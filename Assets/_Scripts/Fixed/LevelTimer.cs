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
    private int lastDisplayedSecond = -1;

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
            // ปัดเศษให้เป็นวินาทีเต็ม
            int currentSecond = Mathf.CeilToInt(currentTime);

            // 🟢 เช็คว่าวินาทีมันเปลี่ยนหรือยัง? ถ้ายังเป็นวินาทีเดิม ไม่ต้องสั่งให้ UI อัปเดตข้อความซ้ำ!
            if (currentSecond != lastDisplayedSecond)
            {
                int minutes = Mathf.FloorToInt(currentSecond / 60F);
                int seconds = Mathf.FloorToInt(currentSecond - minutes * 60);

                timeText.text = string.Format("{0:00}.{1:00}", minutes, seconds);
                lastDisplayedSecond = currentSecond; // จดไว้ว่าอัปเดตวินาทีนี้ไปแล้ว
            }
        }
    }

    private void TimeRanOut()
    {
        Debug.Log("หมดเวลา! กบเขียวขิต!");
        if (player != null)
        {
            // 🟢 โยนหน้าที่สั่งตายกลับไปให้ PlayerController ทำ
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.Die();
            }
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