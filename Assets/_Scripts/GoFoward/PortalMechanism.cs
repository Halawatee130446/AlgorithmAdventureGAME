using System.Collections;
using UnityEngine;

public class PortalMechanism : MonoBehaviour
{
    [Header("จุดวาร์ป")]
    public Transform destinationPoint;

    [Header("สถานะประตู")]
    public bool isUnlocked = false;
    public Animator portalAnimator;

    // ใช้คำสั่ง static เพื่อให้ประตูทุกบานใช้ค่าคูลดาวน์นี้ร่วมกัน
    private static bool isCooldown = false;

    // --- เพิ่มฟังก์ชันนี้เข้ามาเพื่อแก้บั๊ก! ---
    private void Start()
    {
        // บังคับปลดล็อกคูลดาวน์ทุกครั้งที่โหลดเข้าฉากใหม่
        isCooldown = false;
    }
    // ------------------------------------

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
                StartCoroutine(TeleportRoutine(collision.gameObject));
            }
        }
    }

    private IEnumerator TeleportRoutine(GameObject player)
    {
        isCooldown = true;
        player.transform.position = destinationPoint.position;
        yield return new WaitForSeconds(3f);
        isCooldown = false;
    }
}