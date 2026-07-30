using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    [Header("โซนนี้ต้องการกล่อง Tag อะไร?")]
    public string requiredTag;

    private Collider2D dropZoneCollider;
    // เก็บรายการกล่องที่กำลังแตะขอบโซนอยู่
    private List<Collider2D> touchingBoxes = new List<Collider2D>();

    void Start()
    {
        dropZoneCollider = GetComponent<Collider2D>();
    }

    // แค่จดชื่อตอนกล่องเข้ามาแตะ
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(requiredTag))
        {
            if (!touchingBoxes.Contains(collision))
            {
                touchingBoxes.Add(collision);
            }
        }
    }

    // ลบชื่อออกตอนกล่องหลุดขอบ
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(requiredTag))
        {
            if (touchingBoxes.Contains(collision))
            {
                touchingBoxes.Remove(collision);
            }
        }
    }

    // Manager จะเป็นคนมาเรียกใช้ฟังก์ชันนี้ เพื่อขอข้อมูลว่าตอนนี้มีกล่องเข้ามิดกี่ใบ
    public int GetPerfectBoxesCount()
    {
        int count = 0;
        touchingBoxes.RemoveAll(item => item == null); // ล้างค่ากล่องที่อาจถูกลบไปแล้วเพื่อกันบั๊ก

        foreach (Collider2D box in touchingBoxes)
        {
            bool isFullyInside =
                box.bounds.min.x >= dropZoneCollider.bounds.min.x &&
                box.bounds.max.x <= dropZoneCollider.bounds.max.x &&
                box.bounds.min.y >= dropZoneCollider.bounds.min.y &&
                box.bounds.max.y <= dropZoneCollider.bounds.max.y;

            if (isFullyInside)
            {
                count++;
            }
        }
        return count;
    }
}