using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public string mainSceneName = "MainScene";
    public string questionID = "Q1_1";
    public int totalBoxesToSort = 3;

    [Header("Timer Settings")]
    public float timeLimit = 15f;
    public Text timerText;

    private float timer;
    private bool isGameOver = false;

    private int correctlyPlacedBoxes = 0;
    private int attempts; // ตัวแปรนับจำนวนครั้งที่เข้าเล่นข้อนี้

    void Start()
    {
        timer = timeLimit;

        // ดึงจำนวนครั้งที่เคยเล่นข้อนี้มา ถ้าไม่เคยเล่น ค่าจะเป็น 0
        attempts = PlayerPrefs.GetInt(questionID + "_Attempts", 0);

        // บวก 1 แล้วเซฟกลับลงไป (เพื่อให้รู้ว่ากดเข้ามาเล่นแล้วนะ)
        attempts++;
        PlayerPrefs.SetInt(questionID + "_Attempts", attempts);
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
            ProcessGameEnd(false);
        }
    }

    public void AddCorrectlyPlacedBox()
    {
        correctlyPlacedBoxes++;
        CheckWinCondition();
    }

    public void RemoveCorrectlyPlacedBox()
    {
        correctlyPlacedBoxes--;
    }

    private void CheckWinCondition()
    {
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
            Debug.Log("ชนะมินิเกม!");

            // เช็คว่า "เล่นครั้งแรก" (attempts == 1) และ "ยังไม่เคยได้ดาวจากข้อนี้" ใช่ไหม?
            if (attempts == 1 && PlayerPrefs.GetInt(questionID + "_Passed", 0) == 0)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddStar();
                }
            }

            PlayerPrefs.SetInt(questionID + "_Passed", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("แพ้มินิเกม!");
        }

        StartCoroutine(ReturnToMainSceneDelay());
    }

    private IEnumerator ReturnToMainSceneDelay()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(mainSceneName);
    }
}