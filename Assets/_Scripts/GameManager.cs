using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("ระบบเซฟดาว")]
    public int totalStars = 0;

    [Header("ระบบเงิน (สะสมเพื่อ Knowledge Library)")]
    public int globalCoins = 0;

    [Header("ข้อมูลชั่วคราวตอนย้าย Scene")]
    public bool isReturningFromMiniGame = false;
    public Vector2 returnPosition;
    public int savedHealth;
    public int savedAmmo;
    public float savedTime;

    [Header("ระบบ Checkpoint")]
    public bool hasCheckpoint = false; // เช็คว่าเคยเหยียบจุดเช็คอินหรือยัง
    public Vector2 lastCheckpointPos; // เก็บพิกัดจุดเช็คอินล่าสุด

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ล้างเซฟเฉพาะตอนเปิดเกมเล่นครั้งแรก
            PlayerPrefs.DeleteAll();
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddStar()
    {
        totalStars++;
        PlayerPrefs.SetInt("TotalStars", totalStars);
        PlayerPrefs.Save();
    }

    private void LoadGameData()
    {
        totalStars = PlayerPrefs.GetInt("TotalStars", 0);
        globalCoins = PlayerPrefs.GetInt("GlobalCoins", 0);
    }

    // ฟังก์ชันนี้จะถูกเรียกตอนที่กบเขียวตาย
    public void ResetStateOnDeath()
    {
        // 1. ปิดสวิตช์วาร์ปกลับจากมินิเกมให้ชัวร์ 100%
        isReturningFromMiniGame = false;

        // 2. แบ็คอัพดาวไว้ก่อน (เวลาตายดาวจะได้ไม่หาย)
        int keepStars = totalStars;
        int keepCoins = globalCoins;

        // 3. ล้างเซฟทั้งหมด (รีเซ็ตหีบ, มอนสเตอร์, มินิเกม)
        PlayerPrefs.DeleteAll();

        // 4. คืนค่าดาวกลับเข้าไป
        totalStars = keepStars;
        PlayerPrefs.SetInt("TotalStars", totalStars);
        PlayerPrefs.Save();

        globalCoins = keepCoins;
        PlayerPrefs.SetInt("GlobalCoins", globalCoins);

        Debug.Log("ล้างข้อมูลตอนตายเรียบร้อย! เริ่มด่านใหม่แบบคลีนๆ");
    }

    public void AddCoins(int amount)
    {
        globalCoins += amount; // บวกเงินเพิ่มเข้าไป
        PlayerPrefs.SetInt("GlobalCoins", globalCoins); // เซฟลงสมองเกมทันที
        PlayerPrefs.Save();

        Debug.Log("เก็บเหรียญได้! ยอดรวมในบัญชีตอนนี้: " + globalCoins);
    }
}