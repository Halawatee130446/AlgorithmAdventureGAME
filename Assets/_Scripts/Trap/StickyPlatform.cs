using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // ตรวจสอบว่าทั้งตัวสคริปต์/แท่น และ Player ยังเปิดใช้งานอยู่ก่อนเปลี่ยน Parent
            if (gameObject.activeInHierarchy && collision.gameObject.activeInHierarchy)
            {
                collision.gameObject.transform.SetParent(null);
            }
        }
    }

    // กันไว้ในกรณีที่แท่นหรือสคริปต์ถูก Disable/Destroy กะทันหันขณะที่ Player อยู่บนแท่น
    private void OnDisable()
    {
        // หากผู้เล่นยังเป็น Child ของแท่นนี้อยู่ ให้ปลดออกอย่างปลอดภัย
        foreach (Transform child in transform)
        {
            if (child.name == "Player")
            {
                child.SetParent(null);
            }
        }
    }
}

/* เก่าาาาาาาาาาาาาา

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
} */