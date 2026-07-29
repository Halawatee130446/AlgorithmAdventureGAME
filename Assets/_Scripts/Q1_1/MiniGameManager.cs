using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public string mainSceneName = "MainScene";
    public string questionID = "Q1_1";
    public int totalBoxesToSort = 3; // จำนวนกล่องทั้งหมดที่ต้องจัดให้ถูก (เช่น A,B,C มี 3 กล่อง)

    [Header("Timer Settings")]
    public float timeLimit = 15f;
    public Text timerText;

    private float timer;
    private bool isGameOver = false;
    private bool isFirstTry = true;

    // ตัวแปรนับว่าตอนนี้วางถูกกี่กล่องแล้ว
    private int correctlyPlacedBoxes = 0;

    void Start()
    {
        timer = timeLimit;

        if (PlayerPrefs.GetInt(questionID + "_Played", 0) == 1)
        {
            isFirstTry = false;
        }

        PlayerPrefs.SetInt(questionID + "_Played", 1);
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (isGameOver) return;

        timer -= Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString() + "s";
        }

        if (timer <= 0)
        {
            ProcessGameEnd(false); // เวลาหมด = แพ้
        }
    }

    // ฟังก์ชันนี้ DropZone จะเป็นคนเรียกเมื่อกล่องเข้าโซนถูก
    public void AddCorrectlyPlacedBox()
    {
        correctlyPlacedBoxes++;
        CheckWinCondition();
    }

    // ฟังก์ชันนี้ DropZone จะเป็นคนเรียกเมื่อกล่องหลุดออกจากโซน
    public void RemoveCorrectlyPlacedBox()
    {
        correctlyPlacedBoxes--;
    }

    private void CheckWinCondition()
    {
        // ถ้ายอดกล่องที่วางถูก เท่ากับยอดกล่องทั้งหมดที่ต้องจัด แปลว่าชนะ!
        if (correctlyPlacedBoxes >= totalBoxesToSort)
        {
            ProcessGameEnd(true);
        }
    }

    private void ProcessGameEnd(bool isWin)
    {
        if (isGameOver) return;
        isGameOver = true;

        if (isWin)
        {
            Debug.Log("ชนะ! เรียงกล่องถูกหมดแล้ว");
            PlayerPrefs.SetInt(questionID + "_Passed", 1);

            if (isFirstTry)
            {
                int currentStars = PlayerPrefs.GetInt("TotalStars", 0);
                PlayerPrefs.SetInt("TotalStars", currentStars + 1);
                Debug.Log("ได้ดาว 1 ดวง!");
            }
        }
        else
        {
            Debug.Log("แพ้! หมดเวลาก่อน");
        }

        PlayerPrefs.Save();
        StartCoroutine(ReturnToMainSceneDelay());
    }

    private IEnumerator ReturnToMainSceneDelay()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(mainSceneName);
    }
}