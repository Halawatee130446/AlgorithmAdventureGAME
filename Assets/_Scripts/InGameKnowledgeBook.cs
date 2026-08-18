using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameKnowledgeBook : MonoBehaviour
{
    public static InGameKnowledgeBook Instance;

    [Header("UI สมุดรวม")]
    public GameObject bookPanel;
    public Image bookImageDisplay;
    public Text pageNumberText;
    public GameObject nextButton;
    public GameObject prevButton;

    private List<Sprite> collectedPages = new List<Sprite>();
    private int currentPage = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (bookPanel != null) bookPanel.SetActive(false);
    }

    public void AddPages(Sprite[] pages)
    {
        foreach (Sprite page in pages)
        {
            if (!collectedPages.Contains(page))
            {
                collectedPages.Add(page);
            }
        }
    }

    public void OpenBook()
    {
        if (collectedPages.Count == 0) return;

        bookPanel.SetActive(true);
        currentPage = 0;
        Time.timeScale = 0f;
        UpdateBookUI();
    }

    public void CloseBook()
    {
        bookPanel.SetActive(false);
        Time.timeScale = 1f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = true;

            PlayerShooting ps = player.GetComponent<PlayerShooting>();
            if (ps != null) ps.enabled = true;
        }
    }

    // 🟢 แก้ไข: ถ้าอยู่หน้าสุดท้ายแล้ว จะกด Next ไม่ได้
    public void NextPage()
    {
        if (currentPage < collectedPages.Count - 1)
        {
            currentPage++;
            UpdateBookUI();
        }
    }

    // 🟢 แก้ไข: ถ้าอยู่หน้าแรกแล้ว จะกด Prev ไม่ได้
    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateBookUI();
        }
    }

    private void UpdateBookUI()
    {
        if (collectedPages.Count == 0) return;

        // อัปเดตการแสดงรูปภาพ
        if (bookImageDisplay != null)
        {
            bookImageDisplay.sprite = collectedPages[currentPage];
        }

        // อัปเดตเลขหน้า
        if (pageNumberText != null)
        {
            pageNumberText.text = (currentPage + 1) + "/" + collectedPages.Count;
        }

        // 🟢 ระบบจัดการปุ่มอัจฉริยะ (หน้าแรกซ่อนปุ่มย้อน, หน้าสุดท้ายซ่อนปุ่มถัดไป)
        if (prevButton != null)
        {
            prevButton.SetActive(currentPage > 0);
        }

        if (nextButton != null)
        {
            nextButton.SetActive(currentPage < collectedPages.Count - 1);
        }
    }
}