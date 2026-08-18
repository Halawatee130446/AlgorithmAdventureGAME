using UnityEngine;
using UnityEngine.UI;

public class BookUIController : MonoBehaviour
{
    [Header("ใส่ ID หีบทั้งหมดในด่าน (เช่น Chest_1, Chest_2, Chest_3)")]
    public string[] chestIDs; // 🟢 เปลี่ยนจากตัวแปรเดี่ยว เป็น Array เพื่อรับได้หลายชื่อ

    private Animator anim;
    private Button btn;
    private bool isUnlocked = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        btn = GetComponent<Button>();

        btn.onClick.AddListener(OnBookClicked);
    }

    void Update()
    {
        // ถ้าปุ่มยังไม่ถูกปลดล็อก ให้คอยเช็คว่ามีหีบใบไหนโดนเปิดหรือยัง
        if (!isUnlocked)
        {
            foreach (string id in chestIDs)
            {
                if (SaveManager.IsChestOpened(id))
                {
                    isUnlocked = true; // ปลดล็อกปุ่ม
                    if (anim != null)
                    {
                        anim.Play("Book_Unlocked"); // เล่นแอนิเมชันปุ่มเด้ง
                    }
                    break; // ถ้าเจอหีบเปิดแล้ว 1 ใบ ก็ให้หยุดเช็คใบอื่นได้เลย
                }
            }
        }
    }

    void OnBookClicked()
    {
        if (isUnlocked)
        {
            if (InGameKnowledgeBook.Instance != null)
            {
                InGameKnowledgeBook.Instance.OpenBook();
            }
        }
        else
        {
            Debug.Log("หนังสือยังถูกล็อกอยู่! ต้องไปเปิดหีบอย่างน้อย 1 ใบก่อน");
        }
    }
}