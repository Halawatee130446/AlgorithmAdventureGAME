using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    [Header("ด่านปัจจุบัน (ใช้เพื่อปลดล็อกด่านถัดไป)")]
    public int currentLevelIndex = 1; // 🟢 ตั้งค่าใน Inspector ว่านี่คือด่านที่เท่าไหร่ (ด่าน 1 ใส่ 1)

    [Header("UI Panels & Buttons")]
    public GameObject stageClearPanel;
    public GameObject buttonMenu;
    public GameObject buttonNextLV;

    [Header("UI Texts & Values")]
    public Text timeText;
    public Text ammoText;
    public Text coinCollectedText; // 🟢 เพิ่มช่องนี้ไว้ลาก Text เหรียญมาใส่
    public Text resultText;
    public GameObject[] heartIcons;

    [Header("Mini-Game Results UI")]
    public Text q1AttemptText;
    public GameObject q1FilledStar;
    public Text q2AttemptText;
    public GameObject q2FilledStar;
    public Text q3AttemptText;
    public GameObject q3FilledStar;

    [Header("Overall 4-Star System")]
    public float totalLevelTime = 300f;
    public GameObject starPassParent, starPassFilled;
    public GameObject starHealthParent, starHealthFilled;
    public GameObject starTimeParent, starTimeFilled;
    public GameObject starQuizParent, starQuizFilled;

    [Header("Star Conditions")]
    public int requiredHearts = 3;
    public float requiredTimePercent = 0.2f;
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

        if (buttonMenu != null) buttonMenu.SetActive(false);
        if (buttonNextLV != null) buttonNextLV.SetActive(false);
        if (resultText != null) resultText.gameObject.SetActive(false);

        if (starPassParent != null) starPassParent.SetActive(false);
        if (starHealthParent != null) starHealthParent.SetActive(false);
        if (starTimeParent != null) starTimeParent.SetActive(false);
        if (starQuizParent != null) starQuizParent.SetActive(false);

        // 🟢 [ปลดล็อกด่านถัดไป]
        int highestLevelReached = PlayerPrefs.GetInt("LevelReached", 1);
        if (currentLevelIndex + 1 > highestLevelReached)
        {
            PlayerPrefs.SetInt("LevelReached", currentLevelIndex + 1);
            PlayerPrefs.Save();
            Debug.Log("ปลดล็อกด่าน " + (currentLevelIndex + 1) + " เรียบร้อย!");
        }

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

        // 🟢 โชว์เหรียญที่เก็บได้ในตานี้
        if (coinCollectedText != null && GameManager.Instance != null)
        {
            coinCollectedText.text = "x" + GameManager.Instance.sessionCoins.ToString();
        }

        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] != null) heartIcons[i].SetActive(i < currentHearts);
        }

        UpdateMiniGameResultUI("Q1_1", q1AttemptText, q1FilledStar);
        UpdateMiniGameResultUI("Q1_2", q2AttemptText, q2FilledStar);
        UpdateMiniGameResultUI("Q1_3", q3AttemptText, q3FilledStar);

        yield return new WaitForSecondsRealtime(0.5f);

        List<GameObject> passedStars = new List<GameObject>();
        List<GameObject> failedStars = new List<GameObject>();

        if (starPassFilled != null) starPassFilled.SetActive(true);
        passedStars.Add(starPassParent);

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

        List<GameObject> finalOrder = new List<GameObject>();
        finalOrder.AddRange(passedStars);
        finalOrder.AddRange(failedStars);

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

        if (attemptText != null)
        {
            attemptText.text = attempts + " Attempt" + (attempts > 1 ? "s" : "");
        }

        if (filledStarObj != null)
        {
            // 🟢 ดาวเขียวจะโชว์ก็ต่อเมื่อพยายามไม่เกิน 2 ครั้ง ถ้าเกิน 2 (คือ 3) ดาวเขียวจะถูกปิดไป
            filledStarObj.SetActive(attempts <= 2);
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