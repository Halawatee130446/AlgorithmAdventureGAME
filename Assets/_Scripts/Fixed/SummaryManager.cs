using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 🟢 โครงสร้างใหม่: ช่วยให้คุณจัดกลุ่ม UI ควิซได้ง่ายๆ ไม่ต้องสร้างตัวแปรซ้ำๆ
[System.Serializable]
public struct QuizResultUI
{
    public string questionID; // เช่น "Q1_1"
    public Text attemptText;
    public GameObject filledStar;
}

public class SummaryManager : MonoBehaviour
{
    [Header("ด่านปัจจุบัน (ใช้เพื่อปลดล็อกด่านถัดไป)")]
    public int currentLevelIndex = 1;

    [Header("UI Panels & Buttons")]
    public GameObject stageClearPanel;
    public GameObject buttonMenu;
    public GameObject buttonNextLV;

    [Header("UI Texts & Values")]
    public Text timeText;
    public Text ammoText;
    public Text coinCollectedText;
    public Text resultText;
    public GameObject[] heartIcons;

    [Header("Mini-Game Results UI (ตั้งค่าอิสระได้เลย)")]
    // 🟢 เปลี่ยนเป็น Array คุณสามารถใส่ควิซกี่ข้อก็ได้ตามใจชอบใน Inspector
    public QuizResultUI[] quizResults;

    [Header("Overall 4-Star System (เรียงลำดับ: Pass, Health, Time, Quiz)")]
    public GameObject[] starParents; // ลากกรอบดาว (กรอบเปล่า) ทั้ง 4 มาใส่
    public GameObject[] starFilled;  // ลากดาวที่เติมสีทั้ง 4 มาใส่

    [Header("Star Conditions")]
    public int requiredHearts = 3;
    public float requiredTimePercent = 0.2f;
    public float totalLevelTime = 300f;
    public int requiredQuizPass = 2;

    [Header("System References")]
    public HealthSystem playerHealth;
    public LevelTimer levelTimer;
    public PlayerShooting playerShooting;

    [Header("Settings")]
    public string menuSceneName = "Menu";
    public string nextLevelSceneName = "Level_2";

    private void Start()
    {
        if (stageClearPanel != null) stageClearPanel.SetActive(false);
    }

    public void ShowStageClear()
    {
        Time.timeScale = 0f;
        if (stageClearPanel != null) stageClearPanel.SetActive(true);

        ToggleEndGameUI(false); // ปิดปุ่มและดาวไว้ก่อน รอแอนิเมชัน
        UnlockNextLevel();

        StartCoroutine(CalculateAndShowSummaryRoutine());
    }

    // 🟢 แยกฟังก์ชันเปิด/ปิด UI ออกมา เพื่อให้โค้ดหลักสะอาดขึ้น
    private void ToggleEndGameUI(bool isActive)
    {
        if (buttonMenu != null) buttonMenu.SetActive(isActive);
        if (buttonNextLV != null) buttonNextLV.SetActive(isActive);
        if (resultText != null) resultText.gameObject.SetActive(isActive);

        foreach (var parent in starParents)
        {
            if (parent != null) parent.SetActive(false);
        }
    }

    // 🟢 แยกฟังก์ชันปลดล็อกด่านออกมา
    private void UnlockNextLevel()
    {
        int highestLevelReached = PlayerPrefs.GetInt("LevelReached", 1);
        if (currentLevelIndex + 1 > highestLevelReached)
        {
            PlayerPrefs.SetInt("LevelReached", currentLevelIndex + 1);
            PlayerPrefs.Save();
            Debug.Log("ปลดล็อกด่าน " + (currentLevelIndex + 1) + " เรียบร้อย!");
        }
    }

    private IEnumerator CalculateAndShowSummaryRoutine()
    {
        // 1. ดึงข้อมูลดิบมาเตรียมไว้
        int currentHearts = playerHealth != null ? playerHealth.currentHealth : 0;
        int currentAmmo = playerShooting != null ? playerShooting.currentAmmo : 0;
        float currentTime = (levelTimer != null && levelTimer.useTimer) ? levelTimer.GetCurrentTime() : 0f;

        // 2. อัปเดตข้อความทั่วไป
        if (timeText != null) timeText.text = Mathf.FloorToInt(currentTime).ToString() + " s";
        if (ammoText != null) ammoText.text = "x" + currentAmmo.ToString();
        if (coinCollectedText != null && GameManager.Instance != null)
            coinCollectedText.text = "x" + GameManager.Instance.sessionCoins.ToString();

        // 3. อัปเดตไอคอนหัวใจ
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] != null) heartIcons[i].SetActive(i < currentHearts);
        }

        // 4. อัปเดตผลมินิเกม (ใช้ลูปแทน ทำให้รองรับกี่ข้อก็ได้)
        foreach (var quiz in quizResults)
        {
            UpdateMiniGameResultUI(quiz);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // 5. คำนวณเงื่อนไขดาวใหญ่ 4 ดวง
        bool[] starConditions = new bool[4];
        starConditions[0] = true; // ดาวผ่านด่าน (ได้เสมอถ้ามาถึงหน้านี้)
        starConditions[1] = currentHearts >= requiredHearts;
        starConditions[2] = currentTime >= (totalLevelTime * requiredTimePercent);
        starConditions[3] = (GameManager.Instance != null ? GameManager.Instance.totalStars : 0) >= requiredQuizPass;

        int passCount = 0;

        // 6. เล่นแอนิเมชันโชว์ดาวทีละดวง
        for (int i = 0; i < 4; i++)
        {
            if (i >= starParents.Length || i >= starFilled.Length) continue;

            if (starParents[i] != null) starParents[i].SetActive(true);
            if (starFilled[i] != null) starFilled[i].SetActive(starConditions[i]);

            if (starConditions[i]) passCount++;

            yield return new WaitForSecondsRealtime(0.3f);
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // 7. โชว์ผลลัพธ์และเปิดปุ่มกด
        ShowFinalResultText(passCount);
        if (buttonMenu != null) buttonMenu.SetActive(true);
        if (buttonNextLV != null) buttonNextLV.SetActive(true);
    }

    private void ShowFinalResultText(int passCount)
    {
        if (resultText == null) return;

        resultText.gameObject.SetActive(true);
        if (passCount == 4) resultText.text = "Well Done !";
        else if (passCount == 3) resultText.text = "Great !";
        else if (passCount == 2) resultText.text = "Good !";
        else resultText.text = "You Survived !";
    }

    private void UpdateMiniGameResultUI(QuizResultUI quiz)
    {
        if (string.IsNullOrEmpty(quiz.questionID)) return;

        int attempts = PlayerPrefs.GetInt(quiz.questionID + "_Attempts", 1);

        if (quiz.attemptText != null)
        {
            quiz.attemptText.text = attempts + " Attempt" + (attempts > 1 ? "s" : "");
        }

        if (quiz.filledStar != null)
        {
            quiz.filledStar.SetActive(attempts <= 2);
        }
    }

    public void OnClickMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DepositCoinsToSafe();
            GameManager.Instance.ResetStateOnDeath();
        }
        SceneManager.LoadScene(menuSceneName);
    }

    public void OnClickNextLevel()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DepositCoinsToSafe();
            GameManager.Instance.ResetStateOnDeath();
        }
        SceneManager.LoadScene(nextLevelSceneName);
    }
}