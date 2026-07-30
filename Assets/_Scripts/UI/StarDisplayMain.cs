using UnityEngine;

public class StarDisplayMain : MonoBehaviour
{
    [Header("ลากรูปดาวสีสว่าง (Filled Star) มาใส่เรียงตามลำดับ")]
    public GameObject[] filledStars; // Array สำหรับใส่ดาว 3 ดวง

    void Update()
    {
        if (GameManager.Instance != null)
        {
            int currentStars = GameManager.Instance.totalStars;

            // วนลูปเช็คว่าดาวดวงไหนต้องเปิด/ปิด
            for (int i = 0; i < filledStars.Length; i++)
            {
                if (filledStars[i] != null)
                {
                    // ถ้า i น้อยกว่าจำนวนดาวที่มี ให้เปิดโชว์ (true) ถ้าไม่ใช่ให้ซ่อน (false)
                    filledStars[i].SetActive(i < currentStars);
                }
            }
        }
    }
}