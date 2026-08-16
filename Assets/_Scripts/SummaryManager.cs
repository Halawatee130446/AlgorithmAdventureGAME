using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    [Header("UI Panels & Buttons")]
    public GameObject stageClearPanel;
    public GameObject buttonMenu;
    public GameObject buttonNextLV;

    [Header("UI Texts & Values")]
    public Text timeText;
    public Text ammoText;
    public Text resultText;
    public GameObject[] heartIcons;

    [Header("Mini-Game Results UI")]
    public Text q1AttemptText;
    public GameObject q1FilledStar;
    public Text q2AttemptText;
    public GameObject q2FilledStar;
    public Text q3AttemptText;
    public GameObject q3FilledStar;

    [Header("Overall 4-Star System (แบบสลับก้อน GameObject)")]
    public float totalLevelTime = 300f;

    // ดาวดวงที่ 1 (ผ่านด่าน)
    public GameObject starPassParent;
    public GameObject starPassFilled;

    // ดาวดวงที่ 2 (หัวใจ)
    public GameObject starHealthParent;
    public GameObject starHealthFilled;

    // ดาวดวงที่ 3 (เวลา)
    public GameObject starTimeParent;
    public GameObject starTimeFilled;

    // ดาวดวงที่ 4 (ควิซ)
    public GameObject starQuizParent;
    public GameObject starQuizFilled;

    // 🟢 เพิ่มส่วนนี้เข้ามาให้ปรับตั้งค่าเงื่อนไขได้จาก Unity เลย!
    [Header("Star Conditions (ตั้งค่าเงื่อนไขการได้ดาว)")]
    public int requiredHearts = 3; // เลือดขั้นต่ำที่ต้องเหลือ (เช่น ตั้งไว้ 3)
    public float requiredTimePercent = 0.2f; // เปอร์เซ็นต์เวลาที่ต้องเหลือ (0.2 = 20%)
    public int requiredQuizPass = 2; // จำนวนข้อควิซที่ต้องผ่าน (ได้ดาวมินิเกม)

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

        if (buttonMenu != null) buttonMenu.SetActive(false);
        if (buttonNextLV != null) buttonNextLV.SetActive(false);
        if (resultText != null) resultText.gameObject.SetActive(false);

        // ซ่อนก้อนดาวทั้งหมดไว้ก่อน
        if (starPassParent != null) starPassParent.SetActive(false);
        if (starHealthParent != null) starHealthParent.SetActive(false);
        if (starTimeParent != null) starTimeParent.SetActive(false);
        if (starQuizParent != null) starQuizParent.SetActive(false);

        StartCoroutine(CalculateAndShowSummary());
    }

    private IEnumerator CalculateAndShowSummary()
    {
        int currentHearts = playerHealth != null ? playerHealth.currentHealth : 0;
        int currentAmmo = playerShooting != null ? playerShooting.currentAmmo : 0;
        float currentTime = (levelTimer != null && levelTimer.useTimer) ? levelTimer.GetCurrentTime() : 0f;
        int timeInt = Mathf.FloorToInt(currentTime);

        if (timeText != null) timeText.text = timeInt.ToString() + " s";
        if (ammoText != null) ammoText.text = "x" + currentAmmo.ToString();

        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] != null) heartIcons[i].SetActive(i < currentHearts);
        }

        UpdateMiniGameResultUI("Q1_1", q1AttemptText, q1FilledStar);
        UpdateMiniGameResultUI("Q1_2", q2AttemptText, q2FilledStar);
        UpdateMiniGameResultUI("Q1_3", q3AttemptText, q3FilledStar);

        yield return new WaitForSecondsRealtime(0.5f);

        // --- เตรียมจัดเรียงดาว 4 ดวง ---
        List<GameObject> passedStars = new List<GameObject>();
        List<GameObject> failedStars = new List<GameObject>();

        // 1. ดาวผ่านด่าน (ได้เสมอ)
        if (starPassFilled != null) starPassFilled.SetActive(true);
        passedStars.Add(starPassParent);

        // 2. ดาวหัวใจ (เช็คจากค่าที่ตั้งไว้)
        if (currentHearts >= requiredHearts)
        {
            if (starHealthFilled != null) starHealthFilled.SetActive(true);
            passedStars.Add(starHealthParent);
        }
        else
        {
            if (starHealthFilled != null) starHealthFilled.SetActive(false);
            failedStars.Add(starHealthParent);
        }

        // 3. ดาวเวลา (เช็คจากเปอร์เซ็นต์ที่ตั้งไว้)
        float requiredTime = totalLevelTime * requiredTimePercent;
        if (currentTime >= requiredTime)
        {
            if (starTimeFilled != null) starTimeFilled.SetActive(true);
            passedStars.Add(starTimeParent);
        }
        else
        {
            if (starTimeFilled != null) starTimeFilled.SetActive(false);
            failedStars.Add(starTimeParent);
        }

        // 4. ดาวควิซ (เช็คจากจำนวนข้อที่ตั้งไว้)
        int quizStars = GameManager.Instance != null ? GameManager.Instance.totalStars : 0;
        if (quizStars >= requiredQuizPass)
        {
            if (starQuizFilled != null) starQuizFilled.SetActive(true);
            passedStars.Add(starQuizParent);
        }
        else
        {
            if (starQuizFilled != null) starQuizFilled.SetActive(false);
            failedStars.Add(starQuizParent);
        }

        // เอาก้อนที่ผ่าน ขึ้นก่อนก้อนที่พลาด
        List<GameObject> finalOrder = new List<GameObject>();
        finalOrder.AddRange(passedStars);
        finalOrder.AddRange(failedStars);

        // --- แอนิเมชันโชว์ทีละดวง พร้อมสลับที่ ---
        for (int i = 0; i < finalOrder.Count; i++)
        {
            if (finalOrder[i] != null)
            {
                finalOrder[i].transform.SetSiblingIndex(i);
                finalOrder[i].SetActive(true);
            }
            yield return new WaitForSecondsRealtime(0.3f);
        }

        yield return new WaitForSecondsRealtime(0.2f);

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            int passCount = passedStars.Count;
            if (passCount == 4) resultText.text = "Well Done !";
            else if (passCount == 3) resultText.text = "Great !";
            else if (passCount == 2) resultText.text = "Good !";
            else resultText.text = "You Survived !";
        }

        if (buttonMenu != null) buttonMenu.SetActive(true);
        if (buttonNextLV != null) buttonNextLV.SetActive(true);
    }

    private void UpdateMiniGameResultUI(string qID, Text attemptText, GameObject filledStarObj)
    {
        int attempts = PlayerPrefs.GetInt(qID + "_Attempts", 1);
        if (attemptText != null) attemptText.text = attempts + " Attempt" + (attempts > 1 ? "s" : "");
        if (filledStarObj != null) filledStarObj.SetActive(attempts <= 2);
    }

    public void OnClickMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void OnClickNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelSceneName);
    }
}