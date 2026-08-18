using UnityEngine;

// 🟢 static class คือคลาสที่ไม่ต้องนำไปแปะใน GameObject ใดๆ เรียกใช้งานผ่านชื่อ SaveManager ได้เลย
public static class SaveManager
{
    // ==========================================
    // 1. ระบบข้อมูลถาวร (ข้ามด่าน / ไม่โดนลบตอนตาย)
    // ==========================================
    public static int GetTotalStars() => PlayerPrefs.GetInt("TotalStars", 0);
    public static void SetTotalStars(int amount) { PlayerPrefs.SetInt("TotalStars", amount); PlayerPrefs.Save(); }

    public static int GetGlobalCoins() => PlayerPrefs.GetInt("GlobalCoins", 0);
    public static void SetGlobalCoins(int amount) { PlayerPrefs.SetInt("GlobalCoins", amount); PlayerPrefs.Save(); }

    public static int GetLevelReached() => PlayerPrefs.GetInt("LevelReached", 1);
    public static void SetLevelReached(int level) { PlayerPrefs.SetInt("LevelReached", level); PlayerPrefs.Save(); }

    public static int GetExtraHearts() => PlayerPrefs.GetInt("Item_ExtraHeart", 0);
    public static void SetExtraHearts(int amount) { PlayerPrefs.SetInt("Item_ExtraHeart", amount); PlayerPrefs.Save(); }


    // ==========================================
    // 2. ระบบข้อมูลรายด่าน (หีบ, ไอเทม, ควิซ)
    // ==========================================
    public static bool IsChestOpened(string chestID) => PlayerPrefs.GetInt(chestID + "_isOpened", 0) == 1;
    public static void SetChestOpened(string chestID) { PlayerPrefs.SetInt(chestID + "_isOpened", 1); PlayerPrefs.Save(); }

    public static bool IsChestRead(string chestID) => PlayerPrefs.GetInt(chestID + "_hasRead", 0) == 1;
    public static void SetChestRead(string chestID) { PlayerPrefs.SetInt(chestID + "_hasRead", 1); PlayerPrefs.Save(); }

    public static bool IsItemCollected(string itemID) => PlayerPrefs.GetInt(itemID, 0) == 1;
    public static void SetItemCollected(string itemID) { PlayerPrefs.SetInt(itemID, 1); PlayerPrefs.Save(); }

    public static bool IsMiniGamePassed(string questionID) => PlayerPrefs.GetInt(questionID + "_Passed", 0) == 1;
    public static void SetMiniGamePassed(string questionID) { PlayerPrefs.SetInt(questionID + "_Passed", 1); PlayerPrefs.Save(); }

    public static int GetMiniGameAttempts(string questionID) => PlayerPrefs.GetInt(questionID + "_Attempts", 1);
    public static void AddMiniGameAttempt(string questionID)
    {
        int current = GetMiniGameAttempts(questionID);
        PlayerPrefs.SetInt(questionID + "_Attempts", current + 1);
        PlayerPrefs.Save();
    }


    // ==========================================
    // 3. ระบบล้างข้อมูลสุดแกร่ง (กวาดล้างเฉพาะรายด่าน)
    // ==========================================
    public static void ClearLevelDataOnly()
    {
        // 1. จำข้อมูลถาวรไว้ก่อน
        int keepStars = GetTotalStars();
        int keepCoins = GetGlobalCoins();
        int keepLevel = GetLevelReached();
        int keepHearts = GetExtraHearts();

        // 2. ระเบิดเซฟทิ้งทั้งหมด (หีบ, ไอเทม, มอนสเตอร์ จะกลับมาใหม่)
        PlayerPrefs.DeleteAll();

        // 3. คืนค่าข้อมูลถาวรกลับเข้าไป
        SetTotalStars(keepStars);
        SetGlobalCoins(keepCoins);
        SetLevelReached(keepLevel);
        SetExtraHearts(keepHearts);

        Debug.Log("SaveManager: กวาดล้างข้อมูลด่านเรียบร้อย!");
    }

    public static void HardResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("SaveManager: ล้างข้อมูลแบบถอนรากถอนโคน!");
    }
}