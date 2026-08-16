using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameKnowledgeBook : MonoBehaviour
{
    public static InGameKnowledgeBook Instance; // 🟢 ทำให้หีบทุกใบมองเห็นสมุดเล่มนี้ง่ายๆ

    [Header("UI สมุดรวม")]
    public GameObject bookPanel; // หน้าต่างสมุดรวม
    public Text pageNumberText;  // ตัวหนังสือบอกเลขหน้า (เช่น 1/3)
    public GameObject nextButton; // ปุ่มหน้าถัดไป
    public GameObject prevButton; // ปุ่มหน้าก่อนหน้า

    // 🟢 ตัวแปรเก็บหน้ากระดาษ (เรียงตามลำดับที่หีบส่งข้อมูลมาให้)
    private List<GameObject> collectedPages = new List<GameObject>();
    private int currentPage = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (bookPanel != null) bookPanel.SetActive(false);
    }

    // 🟢 ฟังก์ชันนี้หีบสมบัติจะเป็นคนเรียกใช้ เพื่อเอาหน้ากระดาษมายัดใส่สมุด
    public void AddPage(GameObject page)
    {
        if (!collectedPages.Contains(page))
        {
            collectedPages.Add(page);
            page.SetActive(false); // ซ่อนไว้ก่อน ค่อยให้โชว์ตอนเปิดสมุด
        }
    }

    // --- ฟังก์ชันสำหรับปุ่มไอคอนสมุด (เปิดสมุด) ---
    public void OpenBook()
    {
        if (collectedPages.Count == 0)
        {
            Debug.Log("ยังไม่ได้อ่านความรู้เลย สมุดยังว่างเปล่า!");
            return; // ถ้ายังไม่มีหน้ากระดาษ จะไม่ยอมให้เปิดสมุด (หรือคุณจะใส่เสียง Error ก็ได้)
        }

        bookPanel.SetActive(true);
        currentPage = 0; // เปิดมาหน้าแรกเสมอ
        Time.timeScale = 0f; // ฟรีซเวลาเกม
        UpdateBookUI();
    }

    // --- ฟังก์ชันสำหรับปุ่มกากบาทของสมุด (ปิดสมุด) ---
    public void CloseBook()
    {
        bookPanel.SetActive(false);
        Time.timeScale = 1f; // คืนเวลาเกม

        // ซ่อนทุกหน้าเตรียมไว้สำหรับการเปิดครั้งหน้า
        foreach (GameObject page in collectedPages)
        {
            if (page != null) page.SetActive(false);
        }
    }

    public void NextPage()
    {
        if (collectedPages.Count > 1)
        {
            currentPage++;
            if (currentPage >= collectedPages.Count) currentPage = 0;
            UpdateBookUI();
        }
    }

    public void PrevPage()
    {
        if (collectedPages.Count > 1)
        {
            currentPage--;
            if (currentPage < 0) currentPage = collectedPages.Count - 1;
            UpdateBookUI();
        }
    }

    private void UpdateBookUI()
    {
        // โชว์เฉพาะหน้าที่ตรงกับ currentPage
        for (int i = 0; i < collectedPages.Count; i++)
        {
            if (collectedPages[i] != null)
            {
                collectedPages[i].SetActive(i == currentPage);
            }
        }

        if (pageNumberText != null)
        {
            pageNumberText.text = (currentPage + 1) + "/" + collectedPages.Count;
        }

        // ถ้ามีแค่หน้าเดียว ให้ซ่อนปุ่มเลื่อนหน้าซ้ายขวาไปเลย
        bool hasMultiple = collectedPages.Count > 1;
        if (nextButton != null) nextButton.SetActive(hasMultiple);
        if (prevButton != null) prevButton.SetActive(hasMultiple);
    }
}