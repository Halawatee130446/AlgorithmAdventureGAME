using System.Collections;
using UnityEngine;

public class BridgeMechanism : MonoBehaviour
{
    [Header("ส่วนประกอบของสะพาน")]
    public Collider2D bridgeCollider; // กล่อง Collider ที่ทำให้เดินเหยียบได้
    public SpriteRenderer bridgeSprite; // รูปภาพสะพาน

    private void Start()
    {
        // ตอนเริ่มด่าน ให้สะพานปิดการใช้งานไว้ก่อน
        if (bridgeCollider != null) bridgeCollider.enabled = false;
        if (bridgeSprite != null) bridgeSprite.enabled = false;
    }

    // ฟังก์ชันนี้รับสัญญาณมาจาก Unlocker
    public void OpenBridge()
    {
        // แทนที่จะเปิดทันที ให้เรียกฟังก์ชันหน่วงเวลา (Coroutine) แทน
        StartCoroutine(BlinkAndAppearRoutine());
    }

    private IEnumerator BlinkAndAppearRoutine()
    {
        Debug.Log("สะพานกำลังก่อตัว...");

        float blinkDuration = 1.5f; // ระยะเวลากระพริบรวม (1.5 วินาที)
        float blinkInterval = 0.15f; // ความเร็วในการสลับกระพริบ (สลับทุกๆ 0.15 วินาที)
        float timer = 0f;

        // วงลูปนี้จะทำงานไปเรื่อยๆ จนกว่าเวลา (timer) จะครบ 1.5 วินาที
        while (timer < blinkDuration)
        {
            if (bridgeSprite != null)
            {
                // สลับการมองเห็น (ถ้าเปิดอยู่ให้ปิด ถ้าปิดอยู่ให้เปิด)
                bridgeSprite.enabled = !bridgeSprite.enabled;
            }

            // รอเวลา 0.15 วินาที ก่อนไปทำรอบถัดไป
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // เมื่อกระพริบครบ 1.5 วินาทีแล้ว ให้เปิดภาพสะพานค้างไว้ 100%
        if (bridgeSprite != null) bridgeSprite.enabled = true;

        // และสุดท้าย ค่อยเปิด Collider ให้กบเขียวเดินเหยียบได้ (ป้องกันกบกระโดดไปเหยียบตอนสะพานยังประกอบร่างไม่เสร็จ)
        if (bridgeCollider != null) bridgeCollider.enabled = true;

        Debug.Log("สะพานปรากฏสมบูรณ์ พร้อมให้เดินข้าม!");
    }
}