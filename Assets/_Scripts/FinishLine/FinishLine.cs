using System.Collections;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("รหัสคำถามที่ต้องผ่าน (ใส่ให้ครบทั้ง 3 จุด)")]
    // เราใช้ Array เพื่อให้คุณพิมพ์ชื่อ ID คำถามเพิ่มลดได้อิสระใน Inspector
    public string[] requiredQuestionIDs;

    [Header("ดึงระบบสรุปผลมาใส่")]
    public SummaryManager summaryManager;

    private Animator anim;
    private bool isFinished = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        // เล่นแอนิเมชันถ้วยรางวัลแบบปกติรอไว้
        if (anim != null) anim.Play("Finish_idle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้ากบเขียวมาชน และยังไม่ได้เข้าเส้นชัย
        if (collision.CompareTag("Player") && !isFinished)
        {
            // เช็คว่าตอบคำถามครบหรือยัง
            if (CheckAllQuestionsPassed())
            {
                isFinished = true;
                StartCoroutine(FinishSequence());
            }
            else
            {
                Debug.Log("เข้าเส้นชัยไม่ได้! คุณยังตอบคำถามมินิเกมไม่ครบ 3 จุดนะ");
                // (ในอนาคตคุณอาจจะใส่ Text ลอยขึ้นมาเตือนผู้เล่นตรงนี้ได้ครับ)
            }
        }
    }

    // ฟังก์ชันเช็คว่าผ่านครบทุกข้อไหม
    private bool CheckAllQuestionsPassed()
    {
        // วนลูปเช็ค ID คำถามทีละข้อตามที่คุณพิมพ์ไว้ใน Inspector
        foreach (string qID in requiredQuestionIDs)
        {
            // ถ้าเจอข้อไหนที่ค่ายังเป็น 0 (แปลว่ายังไม่ผ่าน)[cite: 1]
            if (PlayerPrefs.GetInt(qID + "_Passed", 0) == 0)
            {
                return false; // เตะออกจากฟังก์ชันและส่งค่าว่า "ยังไม่ผ่าน" ทันที
            }
        }

        return true; // ถ้าผ่านการเช็คด้านบนมาได้หมด แปลว่าผ่านครบ 100%
    }

    private IEnumerator FinishSequence()
    {
        // 1. เล่นแอนิเมชันถ้วยรางวัล (เปลี่ยนจาก idle เป็น Finished)
        if (anim != null) anim.Play("Finished");

        // 2. หน่วงเวลา 0.5 วินาที ให้ผู้เล่นเห็นภาพถ้วยรางวัลขยับก่อน แล้วหน้าต่าง UI ค่อยเด้งขึ้นมาบัง
        yield return new WaitForSeconds(0.5f);

        // 3. เรียกหน้าต่าง Stage Clear ขึ้นมาโชว์!
        if (summaryManager != null)
        {
            summaryManager.ShowStageClear();
        }
    }
}