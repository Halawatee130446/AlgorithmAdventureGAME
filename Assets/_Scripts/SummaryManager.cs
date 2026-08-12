using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    [Header("UI Panels & Buttons")]
    public GameObject stageClearPanel;
    public GameObject buttonMenu;
    public GameObject buttonNextLV;

    [Header("UI Texts (ลาก Text Legacy มาใส่)")]
    public Text timeText;
    public Text heartText;
    public Text ammoText;
    public Text resultText;

    [Header("Stars UI (ลาก FilledStar_1 ของทั้ง 3 ดวงมาใส่ตามลำดับ)")]
    public GameObject[] filledStars; // ใส่ขนาดเป็น 3 แล้วลากดาวที่เติมสีแล้วมาใส่

    [Header("System References")]
    public HealthSystem playerHealth;
    public LevelTimer levelTimer;
    public PlayerShooting playerShooting;

    [Header("Settings")]
    public string menuSceneName = "Menu";
    public string nextLevelSceneName = "Level_2"; // ชื่อซีนด่านถัดไป

    private void Start()
    {
        // ซ่อนหน้าต่างนี้ไว้ก่อนตอนเริ่มเกม
        if (stageClearPanel != null) stageClearPanel.SetActive(false);
    }

    // ฟังก์ชันนี้เรียกใช้เมื่อกบเขียวเข้าเส้นชัย
    public void ShowStageClear()
    {
        Time.timeScale = 0f; // หยุดเวลาในเกม
        if (stageClearPanel != null) stageClearPanel.SetActive(true);

        // ซ่อนปุ่มและข้อความผลลัพธ์ไว้ก่อน ค่อยให้โชว์ตอนท้าย (เพิ่มความตื่นเต้น)
        if (buttonMenu != null) buttonMenu.SetActive(false);
        if (buttonNextLV != null) buttonNextLV.SetActive(false);
        if (resultText != null) resultText.gameObject.SetActive(false);

        // ปิดดาวทุกดวงไว้ก่อน (จะโชว์แค่ StarX สีดำด้านหลัง)
        foreach (GameObject star in filledStars)
        {
            if (star != null) star.SetActive(false);
        }

        StartCoroutine(CalculateAndShowSummary());
    }

    private IEnumerator CalculateAndShowSummary()
    {
        // 1. ดึงข้อมูลปัจจุบันของกบเขียว
        int currentHearts = playerHealth != null ? playerHealth.currentHealth : 0;
        int currentAmmo = playerShooting != null ? playerShooting.currentAmmo : 0;
        float currentTime = (levelTimer != null && levelTimer.useTimer) ? levelTimer.GetCurrentTime() : 0f;
        int timeInt = Mathf.FloorToInt(currentTime);

        // โชว์ตัวเลขทันที (หรือถ้าอยากทำแอนิเมชันตัวเลขวิ่ง ค่อยมาอัปเกรดทีหลังได้)
        if (heartText != null) heartText.text = "x" + currentHearts.ToString();
        if (ammoText != null) ammoText.text = "x" + currentAmmo.ToString();
        if (timeText != null) timeText.text = timeInt.ToString() + " s";

        // หน่วงเวลาให้ผู้เล่นดูตัวเลขแป๊บนึง
        yield return new WaitForSecondsRealtime(0.5f);

        // 2. คำนวณจำนวนดาว (คุณสามารถปรับเงื่อนไขตรงนี้ได้ตามใจชอบ)
        // ตัวอย่างเงื่อนไข: 
        // ได้ 3 ดาว ถ้าเลือดเต็ม 4 ดวง
        // ได้ 2 ดาว ถ้าเลือดเหลือ 2-3 ดวง
        // ได้ 1 ดาว ถ้าเลือดเหลือ 1 ดวง
        // ได้ 0 ดาว ถ้า... (ปกติเข้าเส้นชัยได้แปลว่าเลือดต้องเหลืออย่างน้อย 1 แต่เผื่อไว้)
        int earnedStars = 0;
        if (currentHearts >= 4) earnedStars = 3;
        else if (currentHearts >= 2) earnedStars = 2;
        else if (currentHearts >= 1) earnedStars = 1;

        // ถ้าคุณใช้ระบบดาวจาก GameManager (ดาวจากมินิเกม) ให้ใช้บรรทัดล่างนี้แทนการคำนวณด้านบน:
        // earnedStars = GameManager.Instance != null ? GameManager.Instance.totalStars : 0;

        // 3. แสดงแอนิเมชันเปิดดาวทีละดวง
        for (int i = 0; i < earnedStars; i++)
        {
            if (i < filledStars.Length && filledStars[i] != null)
            {
                filledStars[i].SetActive(true);
                // รอ 0.3 วิ ก่อนเปิดดาวดวงถัดไป ให้จังหวะมันดูตื่นเต้น
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }

        // 4. กำหนดคำพูดใน Result ตามจำนวนดาวที่ได้
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            switch (earnedStars)
            {
                case 3:
                    resultText.text = "Well Done !";
                    resultText.color = Color.green; // เปลี่ยนสีข้อความได้ด้วย
                    break;
                case 2:
                    resultText.text = "Great !";
                    resultText.color = new Color(1f, 0.5f, 0f); // สีส้ม
                    break;
                case 1:
                    resultText.text = "Good !";
                    resultText.color = Color.yellow;
                    break;
                case 0:
                default:
                    resultText.text = "You Survived !";
                    resultText.color = Color.white;
                    break;
            }
        }

        // 5. โชว์ปุ่มให้ไปต่อ
        yield return new WaitForSecondsRealtime(0.2f);
        if (buttonMenu != null) buttonMenu.SetActive(true);
        if (buttonNextLV != null) buttonNextLV.SetActive(true);
    }

    // --- ฟังก์ชันสำหรับปุ่มกด ---
    public void OnClickMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void OnClickNextLevel()
    {
        Time.timeScale = 1f;
        // อย่าลืมเอา Scene ด่าน 2 ไปใส่ใน Build Settings ด้วยนะครับ
        SceneManager.LoadScene(nextLevelSceneName);
    }
}