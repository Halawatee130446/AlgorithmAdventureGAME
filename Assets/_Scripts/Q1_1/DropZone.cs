using UnityEngine;

public class DropZone : MonoBehaviour
{
    [Header("โซนนี้ต้องการกล่อง Tag อะไร?")]
    public string requiredTag; // ใส่ "CorrectBox" สำหรับโซนเขียว, "WrongBox" สำหรับโซนแดง

    public MiniGameManager gameManager; // ลาก GameManager มาใส่ช่องนี้

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้ากล่องที่ดันเข้ามา มี Tag ตรงกับที่โซนนี้ต้องการ
        if (collision.CompareTag(requiredTag))
        {
            gameManager.AddCorrectlyPlacedBox(); // บอก Manager ว่าบวก 1
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ถ้าผู้เล่นดันกล่องที่ถูก ดันทะลุออกไปนอกโซน
        if (collision.CompareTag(requiredTag))
        {
            gameManager.RemoveCorrectlyPlacedBox(); // บอก Manager ว่าลบ 1
        }
    }
}