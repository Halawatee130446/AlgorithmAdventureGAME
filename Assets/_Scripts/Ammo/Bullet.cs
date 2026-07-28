using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f; // ความเร็วกระสุน
    [SerializeField] private float lifeTime = 0.5f; // เวลาจำกัดก่อนกระสุนหายไป (ระยะยิง)

    private Rigidbody2D rb;

    // ฟังก์ชันนี้จะถูกเรียกจาก Player ตอนกดยิง เพื่อบอกว่าให้พุ่งไปซ้ายหรือขวา
    public void SetDirection(float direction)
    {
        rb = GetComponent<Rigidbody2D>();

        // สั่งให้กระสุนพุ่งไปตามทิศทาง
        rb.velocity = new Vector2(direction * speed, 0f);

        // ถ้าหันซ้าย ให้พลิกรูปกระสุนด้วย (เผื่อรูปกระสุนของคุณมีหัวแหลมๆ)
        if (direction < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }

        // สั่งทำลายกระสุนตัวเองเมื่อครบเวลา lifeTime (จำกัดระยะยิง)
        Destroy(gameObject, lifeTime);
    }

    // เมื่อกระสุนไปชนอะไรสักอย่าง (ที่ติ๊ก/ไม่ได้ติ๊ก Is Trigger ก็ตาม)
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. ถ้าชนมอนสเตอร์
        if (hitInfo.CompareTag("Monster"))
        {
            MonsterHealth monster = hitInfo.GetComponent<MonsterHealth>();
            if (monster != null)
            {
                monster.TakeDamage(); // สั่งลดเลือดมอนสเตอร์
            }
            Destroy(gameObject); // กระสุนทำลายตัวเองทันที
        }
        // 2. ถ้าชนพื้นหรือกำแพง ให้กระสุนหายไปเลย (อย่าลืมตั้ง Tag พื้นเป็น Ground นะครับ)
        else if (hitInfo.CompareTag("Ground") || hitInfo.CompareTag("Trap"))
        {
            Destroy(gameObject);
        }
    }
}