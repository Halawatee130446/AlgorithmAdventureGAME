using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("ระบบเซฟดาว")]
    public int totalStars = 0;

    [Header("ระบบเงิน (แยกกระเป๋าตังค์กับตู้เซฟ)")]
    public int globalCoins = 0;
    public int sessionCoins = 0;

    private List<string> collectedCoinsInSession = new List<string>();

    [Header("ข้อมูลชั่วคราวตอนย้าย Scene")]
    public bool isReturningFromMiniGame = false;
    public Vector2 returnPosition;
    public int savedHealth;
    public int savedAmmo;
    public float savedTime;

    [Header("ระบบ Checkpoint")]
    public bool hasCheckpoint = false;
    public Vector2 lastCheckpointPos;

    // 🟢 เพิ่มตัวแปรนี้เข้ามา เพื่อให้มันจำว่าตานี้กดใช้ไอเทมมาจากหน้า LevelSelect หรือเปล่า
    [Header("ระบบไอเทมเสริม (Boosters)")]
    public bool isUsingExtraHeart = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ⚠️ (ถ้าอยากให้เซฟเป็นอมตะเวลาปิดเกม อย่าลืมใส่ // หน้า PlayerPrefs.DeleteAll(); นะครับ!)
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

    public void AddCoins(int amount, string coinID)
    {
        sessionCoins += amount;
        if (!string.IsNullOrEmpty(coinID) && !collectedCoinsInSession.Contains(coinID))
        {
            collectedCoinsInSession.Add(coinID);
        }
    }

    public bool IsCoinCollected(string coinID)
    {
        return collectedCoinsInSession.Contains(coinID);
    }

    public int GetTotalCoinsForUI()
    {
        return globalCoins + sessionCoins;
    }

    public void DropWallet()
    {
        sessionCoins = 0;
        collectedCoinsInSession.Clear();
    }

    public void DepositCoinsToSafe()
    {
        globalCoins += sessionCoins;
        sessionCoins = 0;
        collectedCoinsInSession.Clear();

        PlayerPrefs.SetInt("GlobalCoins", globalCoins);
        PlayerPrefs.Save();
    }

    // 🟢 ระบบล้างด่านสุดแกร่งที่อัปเกรดแล้ว
    public void ResetStateOnDeath()
    {
        isReturningFromMiniGame = false;
        hasCheckpoint = false;
        lastCheckpointPos = Vector2.zero;

        DropWallet(); // ทำกระเป๋าตังค์หก และรีเซ็ตเหรียญบนแมพ

        // 🟢 1. สำรองข้อมูลสำคัญระดับ Global ไว้ก่อน (เพื่อไม่ให้หายตอน DeleteAll)
        int keepStars = totalStars;
        int keepCoins = globalCoins;
        int keepLevelReached = PlayerPrefs.GetInt("LevelReached", 1);
        int keepExtraHeart = PlayerPrefs.GetInt("Item_ExtraHeart", 0);

        // 🟢 2. ล้างประวัติภายในด่านนี้ทิ้งทั้งหมด (หีบกลับมาปิด, มอนสเตอร์เกิดใหม่, ควิซรีเซ็ต)
        PlayerPrefs.DeleteAll();

        // 🟢 3. คืนค่าข้อมูลสำคัญกลับเข้าสู่ระบบ
        totalStars = keepStars;
        PlayerPrefs.SetInt("TotalStars", totalStars);

        globalCoins = keepCoins;
        PlayerPrefs.SetInt("GlobalCoins", globalCoins);

        PlayerPrefs.SetInt("LevelReached", keepLevelReached);
        PlayerPrefs.SetInt("Item_ExtraHeart", keepExtraHeart);

        PlayerPrefs.Save();

        Debug.Log("กวาดล้างข้อมูลด่านเรียบร้อย! หีบสมบัติ/มอนสเตอร์ ถูกรีเซ็ต 100%");
    }
}