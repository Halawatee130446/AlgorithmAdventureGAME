using UnityEngine;
using UnityEngine.UI;

public class CustomProgressBar : MonoBehaviour
{
    [Header("อ้างอิงตัวละคร")]
    public Transform player; // ลากกบเขียว (Player) มาใส่

    [Header("จุดอ้างอิงในเกม (World Transforms)")]
    public Transform worldStart;       // ลากจุด SafeSpot จุดแรกสุดมาใส่
    public Transform[] worldCheckpoints; // ปรับ Size เป็น 3 แล้วลากเสา Checkpoint 1, 2, 3 ในด่านมาใส่
    public Transform worldGoal;        // ลากถ้วยรางวัล FinishLine ในด่านมาใส่

    [Header("จุดอ้างอิงบน UI (Canvas RectTransforms)")]
    public RectTransform uiFrog;       // ลาก UI รูป Frog มาใส่
    public RectTransform uiStart;      // ลาก UI MarkPoint (Start) มาใส่
    public RectTransform[] uiCheckpoints; // ปรับ Size เป็น 3 แล้วลาก UI MarkPoint (1), (2), (3) มาใส่
    public RectTransform uiGoal;       // ลาก UI MarkPoint (FinishLine) มาใส่

    // ตัวแปรซ่อนไว้ใช้คำนวณเบื้องหลัง
    private Transform[] allWorldPoints;
    private RectTransform[] allUIPoints;

    void Start()
    {
        // รวบรวมจุดในเกมทั้งหมด (รวม 5 จุด: Start -> 1 -> 2 -> 3 -> Goal)
        allWorldPoints = new Transform[5];
        allWorldPoints[0] = worldStart;
        for (int i = 0; i < 3; i++) allWorldPoints[i + 1] = worldCheckpoints[i];
        allWorldPoints[4] = worldGoal;

        // รวบรวมจุดใน UI ทั้งหมดให้ตรงกัน
        allUIPoints = new RectTransform[5];
        allUIPoints[0] = uiStart;
        for (int i = 0; i < 3; i++) allUIPoints[i + 1] = uiCheckpoints[i];
        allUIPoints[4] = uiGoal;
    }

    void Update()
    {
        if (player == null) return;

        float playerX = player.position.x;
        int currentSegment = 0;

        // 1. หาว่าตอนนี้กบเขียวเดินอยู่ระหว่างช่วงไหน (Segment)
        for (int i = 0; i < allWorldPoints.Length - 1; i++)
        {
            if (playerX >= allWorldPoints[i].position.x && playerX <= allWorldPoints[i + 1].position.x)
            {
                currentSegment = i;
                break;
            }
            // ถ้าวิ่งทะลุเส้นชัยไปแล้ว ให้คาบอยู่ช่วงสุดท้าย
            else if (playerX > allWorldPoints[allWorldPoints.Length - 1].position.x)
            {
                currentSegment = allWorldPoints.Length - 2;
            }
        }

        // 2. ดึงค่าตำแหน่ง X ของช่วงปัจจุบันในด่านมาคำนวณ
        float startWorldX = allWorldPoints[currentSegment].position.x;
        float endWorldX = allWorldPoints[currentSegment + 1].position.x;

        if (endWorldX - startWorldX == 0) return; // กัน Error หารด้วยศูนย์

        // 3. คำนวณเปอร์เซ็นต์ว่าเดินมากี่เปอร์เซ็นต์ของช่วงนี้แล้ว (ได้ค่า 0.0 ถึง 1.0)
        float t = (playerX - startWorldX) / (endWorldX - startWorldX);
        t = Mathf.Clamp01(t); // บังคับไม่ให้ค่าน้อยกว่า 0 หรือมากกว่า 1

        // 4. เอาเปอร์เซ็นต์ (t) ไปขยับกบเขียวบนหน้าจอ UI
        Vector2 startUI = allUIPoints[currentSegment].anchoredPosition;
        Vector2 endUI = allUIPoints[currentSegment + 1].anchoredPosition;

        uiFrog.anchoredPosition = Vector2.Lerp(startUI, endUI, t);
    }
}