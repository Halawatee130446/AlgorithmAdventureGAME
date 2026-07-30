using UnityEngine;
using UnityEngine.UI; // ต้องใช้เพื่อควบคุม Button

public class BookUIController : MonoBehaviour
{
    [Header("Settings")]
    public string chestID = "Chest_1"; // ชื่อต้องตรงกับหีบที่จะปลดล็อก
    public GameObject knowledgePanel; // ลากหน้าต่าง Knowledge มาใส่ช่องนี้

    private Animator anim;
    private Button btn;
    private bool isUnlocked = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        btn = GetComponent<Button>();

        // ผูกฟังก์ชันเข้ากับการกดปุ่ม
        btn.onClick.AddListener(OnBookClicked);
    }

    void Update()
    {
        // เช็คตลอดเวลาว่าหีบถูกเปิดหรือยัง และถ้ายังไม่เคยเปลี่ยนสถานะปุ่ม ให้ทำการเปลี่ยน
        if (!isUnlocked && PlayerPrefs.GetInt(chestID + "_isOpened", 0) == 1)
        {
            isUnlocked = true;
            if (anim != null)
            {
                anim.Play("Book_Unlocked"); // สั่งเล่นท่าปลดล็อก (ชื่อต้องตรงกับใน Animator)
            }
        }
    }

    // ฟังก์ชันนี้จะทำงานเมื่อผู้เล่นคลิกปุ่ม
    void OnBookClicked()
    {
        if (isUnlocked) // ถ้าปลดล็อกแล้ว ให้เปิดหน้าต่างความรู้
        {
            if (knowledgePanel != null)
            {
                knowledgePanel.SetActive(true);
                // Tip: ถ้าอยากให้เกมหยุดชั่วคราวตอนอ่านหนังสือ สามารถใส่ Time.timeScale = 0f; ได้ครับ
            }
        }
        else // ถ้ายังไม่ปลดล็อก
        {
            Debug.Log("หนังสือยังถูกล็อกอยู่! ต้องไปเปิดหีบก่อน");
        }
    }
}