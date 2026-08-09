using System.Collections;
using UnityEngine;

public class PortalMechanism : MonoBehaviour
{
    [Header("จุดวาร์ป")]
    public Transform destinationPoint;

    [Header("สถานะประตู")]
    public bool isUnlocked = false;
    public Animator portalAnimator;

    // ใช้คำสั่ง static เพื่อให้ประตูทุกบานใช้ค่าคูลดาวน์นี้ "ร่วมกัน"
    private static bool isCooldown = false;

    public void UnlockPortal()
    {
        isUnlocked = true;
        if (portalAnimator != null)
        {
            portalAnimator.Play("Teleport_On");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // เช็คว่าไม่ใช่ช่วงคูลดาวน์ ถึงจะยอมให้วาร์ป
        if (collision.CompareTag("Player") && isUnlocked && !isCooldown)
        {
            if (destinationPoint != null)
            {
                // เรียกใช้ Coroutine เพื่อให้สั่งหน่วงเวลาได้
                StartCoroutine(TeleportRoutine(collision.gameObject));
            }
        }
    }

    // ฟังก์ชัน Coroutine สำหรับจัดการการวาร์ปและหน่วงเวลา
    private IEnumerator TeleportRoutine(GameObject player)
    {
        // 1. ล็อกประตูทุกบานบนกระดานไม่ให้ทำงาน
        isCooldown = true;

        // 2. ย้ายตำแหน่งตัวละครกบเขียวไปที่เป้าหมาย
        player.transform.position = destinationPoint.position;
        Debug.Log("วาร์ปสำเร็จ! กำลังติดคูลดาวน์ 3 วินาที...");

        // 3. ยืนค้างหน่วงเวลา 3 วินาที (เปลี่ยนตัวเลขตรงนี้ได้ตามต้องการ)
        yield return new WaitForSeconds(3f);

        // 4. หมดเวลาคูลดาวน์ ปลดล็อกให้ประตูทำงานได้อีกครั้ง
        isCooldown = false;
        Debug.Log("คูลดาวน์เสร็จสิ้น ประตูพร้อมใช้งาน!");
    }
}