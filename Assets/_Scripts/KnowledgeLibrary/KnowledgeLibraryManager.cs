using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KnowledgeLibraryManager : MonoBehaviour
{
    [Header("ตั้งค่าชื่อซีนเมนูหลัก")]
    public string menuSceneName = "Menu"; // เปลี่ยนให้ตรงกับชื่อซีน Menu ของคุณ

    [Header("UI โชว์เหรียญสะสม (ตู้เซฟ)")]
    public Text globalCoinText; // ลาก Text ที่จะใช้โชว์เหรียญสะสมมาใส่ช่องนี้

    private void Start()
    {
        // พอเปิดหน้าห้องสมุดปุ๊บ ให้อัปเดตตัวเลขเหรียญทันที
        UpdateGlobalCoinUI();
    }

    // --- ฟังก์ชันอัปเดต UI เหรียญสะสม ---
    public void UpdateGlobalCoinUI()
    {
        if (globalCoinText != null && GameManager.Instance != null)
        {
            // 🟢 ดึงยอดเงิน "ตู้เซฟถาวร" (globalCoins) มาโชว์ (ไม่ใช่เงินชั่วคราวในกระเป๋า)
            globalCoinText.text = "x " + GameManager.Instance.globalCoins.ToString();
        }
        else if (globalCoinText != null)
        {
            // เผื่อเปิดซีนนี้เดี่ยวๆ ตอนเทสต์ แล้วหา GameManager ไม่เจอ
            globalCoinText.text = "x 0";
        }
    }

    // --- ฟังก์ชันสำหรับปุ่มย้อนกลับ (Back) ---
    public void ClickBackToMenu()
    {
        // วาร์ปกลับหน้าเมนูหลัก
        SceneManager.LoadScene(menuSceneName);
    }
}