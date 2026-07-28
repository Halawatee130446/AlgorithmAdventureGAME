using UnityEngine;
using UnityEngine.UI; // สำคัญมาก! ต้องมีบรรทัดนี้ถึงจะสั่งงานรูปภาพ UI ได้

public class HealthUI : MonoBehaviour
{
    // สร้างกล่องเพื่อรอรับรูปหัวใจทั้ง 3 ดวงจากหน้า Editor
    [SerializeField] private Image[] hearts;

    // ฟังก์ชันนี้จะถูกเรียกใช้เมื่อเลือดลด
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                // ถ้าตำแหน่งของรูปน้อยกว่าเลือดที่เหลือ ให้แสดงรูปหัวใจ
                hearts[i].enabled = true;
            }
            else
            {
                // ถ้าตำแหน่งของรูปมากกว่าหรือเท่ากับเลือดที่เหลือ ให้ซ่อนรูปหัวใจ
                hearts[i].enabled = false;
            }
        }
    }
}