using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public string questionID = "Q1_1";
    public string mainSceneName = "MainScene";
    public float timeLimit = 60f;

    [Header("Mini-Game Logic (ระบบนับกล่อง)")]
    // --- เปลี่ยนมาใช้ Array เพื่อเก็บโซนทั้งหมด ---
    public DropZone[] allDropZones;
    public int totalBoxes = 3;

    private bool isWinning = false;
    private Coroutine winCoroutine;

    [Header("Pre-Game UI (ก่อนเริ่ม)")]
    public GameObject tutorialPanel;

    [Header("In-Game UI (ระหว่างเล่น)")]
    public Text inGameTimerText;

    [Header("Post-Game UI (หลังจบเกม)")]
    public GameObject summaryPanel;
    public Text resultTitleText;
    public Text timeSpentText;
    public Text attemptSummaryText;
    public GameObject brightStar;
    public GameObject darkStar;

    [Header("Buttons (ปุ่มกดหลังจบ)")]
    public GameObject closeButton;
    public GameObject retryButton;
    public GameObject goBackButton;

    private int attempts = 1;
    private bool isPlaying = false;
    private bool isGameOver = false;
    private float timeSpent = 0f;

    void Start()
    {
        attempts = PlayerPrefs.GetInt(questionID + "_Attempts", 1);

        Time.timeScale = 0f;
        tutorialPanel.SetActive(true);
        summaryPanel.SetActive(false);

        UpdateInGameTimerUI();
    }

    void Update()
    {
        if (isPlaying && !isGameOver && !isWinning)
        {
            timeSpent += Time.deltaTime;
            timeLimit -= Time.deltaTime;

            UpdateInGameTimerUI();

            if (timeLimit <= 0)
            {
                timeLimit = 0;
                UpdateInGameTimerUI();
                ProcessGameEnd(false);
            }

            // ให้ Manager ทำการกวาดสายตาเช็คภาพรวมตลอดเวลาที่เล่น
            CheckOverallWinCondition();
        }
    }

    private void UpdateInGameTimerUI()
    {
        if (inGameTimerText != null)
        {
            int min = Mathf.FloorToInt(timeLimit / 60F);
            int sec = Mathf.FloorToInt(timeLimit - min * 60);
            inGameTimerText.text = string.Format(": {0:00}:{1:00}", min, sec);
        }
    }

    public void StartMiniGame()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        isPlaying = true;
    }

    public void OnTutorialCloseClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainSceneName);
    }

    // --- ฟังก์ชันเช็คภาพรวมทั้งหมด ---
    public void CheckOverallWinCondition()
    {
        int currentPerfectBoxes = 0;

        // วนถาม DropZone ทุกโซนบนกระดาน ว่ามีกล่องที่เข้าเป้าสมบูรณ์กี่ใบ
        foreach (DropZone zone in allDropZones)
        {
            if (zone != null)
            {
                currentPerfectBoxes += zone.GetPerfectBoxesCount();
            }
        }

        // ถ้ายอดรวมกล่องที่เข้าเป้า เท่ากับจำนวนที่ต้องการ
        if (currentPerfectBoxes >= totalBoxes && !isWinning && !isGameOver)
        {
            winCoroutine = StartCoroutine(WinDelayRoutine());
        }
        else if (currentPerfectBoxes < totalBoxes && isWinning)
        {
            // ถ้ายกกล่องออกระหว่างนับถอยหลัง 1.2 วิ ให้ยกเลิกการชนะ
            isWinning = false;
            if (winCoroutine != null) StopCoroutine(winCoroutine);
        }
    }

    private IEnumerator WinDelayRoutine()
    {
        isWinning = true;
        yield return new WaitForSeconds(1.2f); // รอ 1.2 วินาที

        // เช็คย้ำอีกรอบตอนครบ 1.2 วิ เพื่อความชัวร์ว่าผู้เล่นไม่ได้แอบดึงกล่องออกตอนวิสุดท้าย
        int finalCheck = 0;
        foreach (DropZone zone in allDropZones)
        {
            if (zone != null) finalCheck += zone.GetPerfectBoxesCount();
        }

        if (finalCheck >= totalBoxes)
        {
            ProcessGameEnd(true);
        }
        else
        {
            isWinning = false;
        }
    }

    private void ProcessGameEnd(bool isWin)
    {
        if (isGameOver) return;
        isGameOver = true;
        isPlaying = false;
        isWinning = false;

        Time.timeScale = 0f;
        summaryPanel.SetActive(true);

        int minutes = Mathf.FloorToInt(timeSpent / 60F);
        int seconds = Mathf.FloorToInt(timeSpent - minutes * 60);
        timeSpentText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (isWin)
        {
            resultTitleText.text = "STAGE CLEAR!";
            closeButton.SetActive(true);
            retryButton.SetActive(false);
            goBackButton.SetActive(false);

            if (attempts == 1 && PlayerPrefs.GetInt(questionID + "_Passed", 0) == 0)
            {
                attemptSummaryText.text = "Good Job!";
                brightStar.SetActive(true);
                darkStar.SetActive(false);
                if (GameManager.Instance != null) GameManager.Instance.AddStar();
            }
            else
            {
                attemptSummaryText.text = attempts + " attempts";
                brightStar.SetActive(false);
                darkStar.SetActive(true);
            }

            PlayerPrefs.SetInt(questionID + "_Passed", 1);
            PlayerPrefs.Save();
        }
        else
        {
            resultTitleText.text = "TIME'S UP!";
            attemptSummaryText.text = "Try again!";
            brightStar.SetActive(false);
            darkStar.SetActive(true);

            closeButton.SetActive(false);
            retryButton.SetActive(true);
            goBackButton.SetActive(true);
        }
    }

    public void OnCloseClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainSceneName);
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f;
        attempts++;
        PlayerPrefs.SetInt(questionID + "_Attempts", attempts);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnGoBackClicked()
    {
        Time.timeScale = 1f;
        attempts++;
        PlayerPrefs.SetInt(questionID + "_Attempts", attempts);
        PlayerPrefs.Save();

        SceneManager.LoadScene(mainSceneName);
    }
}