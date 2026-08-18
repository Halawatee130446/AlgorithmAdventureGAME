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

    // 🟢 ระบบล้างด่านสุดแกร่ง
    public void ResetStateOnDeath()
    {
        isReturningFromMiniGame = false;
        hasCheckpoint = false;
        lastCheckpointPos = Vector2.zero;

        DropWallet();

        // 🟢 เรียกใช้ SaveManager บรรทัดเดียวจบ! โค้ดสะอาดขึ้น 10 เท่า
        SaveManager.ClearLevelDataOnly();
    }

    // ---------------------------------------------------
    // 🟢 โค้ดที่เพิ่มใหม่จาก Action Plan 2: ศูนย์กลางการเซฟข้อมูลก่อนไปมินิเกม
    // ---------------------------------------------------
    public void SavePlayerStateBeforeMiniGame()
    {
        isReturningFromMiniGame = true;

        // 1. หาตัวกบเขียวและเซฟ เลือด/กระสุน/พิกัด
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            returnPosition = player.transform.position;

            HealthSystem hs = player.GetComponent<HealthSystem>();
            if (hs != null) savedHealth = hs.currentHealth;

            PlayerShooting ps = player.GetComponent<PlayerShooting>();
            if (ps != null) savedAmmo = ps.currentAmmo;
        }

        // 2. หาระบบเวลาในฉาก และเซฟเวลาที่เหลือ
        LevelTimer levelTimer = Object.FindFirstObjectByType<LevelTimer>();
        if (levelTimer != null && levelTimer.useTimer)
        {
            savedTime = levelTimer.GetCurrentTime();
        }

        Debug.Log("GameManager: บันทึกข้อมูลผู้เล่นเรียบร้อย เตรียมเปิดมินิเกม!");
    }
}