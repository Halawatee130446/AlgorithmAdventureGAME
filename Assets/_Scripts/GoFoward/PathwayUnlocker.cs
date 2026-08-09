using UnityEngine;
using UnityEngine.Events; // ต้องใช้ตัวนี้เพื่อทำระบบลากวางออบเจค (UnityEvent)

public class PathwayUnlocker : MonoBehaviour
{
    [Header("รหัสคำถามที่ต้องตอบถูกเพื่อปลดล็อก")]
    public string questionID = "Q1_1";

    [Header("สิ่งที่จะเกิดขึ้นเมื่อปลดล็อกทางเชื่อม (ลากสคริปต์มาใส่ที่นี่)")]
    public UnityEvent onPathwayUnlocked;

    private bool isUnlocked = false;

    void Start()
    {
        CheckPathwayStatus();
    }

    // เราอาจจะทำปุ่มทดสอบชั่วคราว หรือใช้ Update เช็คตอนกลับมาจากซีนมินิเกมก็ได้
    void Update()
    {
        // เช็คเผื่อกรณีที่มีการปลดล็อกระหว่างที่อยู่ในซีนนี้
        if (!isUnlocked)
        {
            CheckPathwayStatus();
        }
    }

    private void CheckPathwayStatus()
    {
        // ดึงค่าเซฟที่ MiniGameManager บันทึกไว้ (1 = ผ่านแล้ว, 0 = ยังไม่ผ่าน)
        if (PlayerPrefs.GetInt(questionID + "_Passed", 0) == 1)
        {
            isUnlocked = true;

            // สั่งให้ Event ทำงาน (ใครก็ตามที่โดนลากมาใส่ใน Inspector จะโดนสั่งงานหมด)
            onPathwayUnlocked.Invoke();
        }
    }
}