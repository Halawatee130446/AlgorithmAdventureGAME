using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HowToPlayManager : MonoBehaviour
{
    [Header("ตั้งค่าซีน")]
    public string mainMenuScene = "Menu"; // 🟢 เปลี่ยนเป็น Menu ตามที่แจ้งแล้วครับ

    [Header("ตั้งค่าหน้ากระดาษ")]
    public GameObject[] pages; // ลาก Panel ของแต่ละหน้า (1, 2, 3...) มาใส่ตรงนี้

    [Header("UI บอกเลขหน้า")]
    public Text pageNumberText; // ลาก Text ที่จะใช้โชว์ "1/3" มาใส่

    private int currentPage = 0; // หน้าปัจจุบัน (คอมพิวเตอร์เริ่มนับจาก 0)

    void Start()
    {
        UpdatePageUI();
    }

    // --- ฟังก์ชันปุ่มเลื่อนขวา (Next) ---
    public void NextPage()
    {
        currentPage++;

        // ถ้ายกหน้าเกินจำนวนที่มี ให้วนกลับไปหน้าแรก (เลื่อนไม่จำกัด)
        if (currentPage >= pages.Length)
        {
            currentPage = 0;
        }

        UpdatePageUI();
    }

    // --- ฟังก์ชันปุ่มเลื่อนซ้าย (Previous) ---
    public void PreviousPage()
    {
        currentPage--;

        // ถ้าถอยหลังจนติดลบ ให้วนไปหน้าสุดท้าย (เลื่อนไม่จำกัด)
        if (currentPage < 0)
        {
            currentPage = pages.Length - 1;
        }

        UpdatePageUI();
    }

    // --- ฟังก์ชันอัปเดตหน้าจอและตัวหนังสือ ---
    private void UpdatePageUI()
    {
        // 1. ซ่อนทุกหน้า แล้วเปิดเฉพาะหน้าที่ตรงกับ currentPage
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == currentPage);
            }
        }

        // 2. อัปเดตตัวหนังสือบอกเลขหน้า
        if (pageNumberText != null)
        {
            // +1 เพราะต้องโชว์ให้คนดู (คนเริ่มนับจาก 1)
            pageNumberText.text = (currentPage + 1).ToString() + "/" + pages.Length.ToString();
        }
    }

    // --- ฟังก์ชันปุ่มย้อนกลับไปเมนู ---
    public void ClickBackToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}