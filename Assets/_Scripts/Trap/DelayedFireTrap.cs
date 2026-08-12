using System.Collections;
using UnityEngine;

public class DelayedFireTrap : MonoBehaviour
{
    [Header("ตั้งค่าการทำงาน")]
    public float delayTime = 0.8f; // เวลาหน่วงก่อนไฟพุ่ง (ให้ผู้เล่นวิ่งผ่าน)
    public float fireDuration = 1.5f; // ระยะเวลาที่ไฟติดค้างไว้

    [Header("ส่วนประกอบ")]
    public GameObject fireHitbox; // ลาก Object ลูกที่ชื่อ FireHitbox มาใส่ช่องนี้

    private Animator anim;
    private bool isActivated = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        // ตอนเริ่มเกม บังคับปิดกล่องดาเมจไฟไว้ก่อน
        if (fireHitbox != null) fireHitbox.SetActive(false);
    }

    // ฟังก์ชันนี้ทำงานเมื่อกบเขียวเดินมาเหยียบฐาน (Collider แข็ง)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isActivated)
        {
            StartCoroutine(ActivateFireRoutine());
        }
    }

    private IEnumerator ActivateFireRoutine()
    {
        isActivated = true; // ล็อกไว้ไม่ให้เหยียบซ้ำจนรวน

        // 1. รอหน่วงเวลา (จังหวะนี้กบเขียวต้องรีบวิ่งให้พ้น!)
        yield return new WaitForSeconds(delayTime);

        // 2. ไฟติด! (เปลี่ยนชื่อแอนิเมชันเป็น GroundFire_On)
        if (anim != null) anim.Play("GroundFire_On");
        if (fireHitbox != null) fireHitbox.SetActive(true);

        // 3. รอเวลาให้ไฟลุกไหม้
        yield return new WaitForSeconds(fireDuration);

        // 4. ไฟดับ! (เปลี่ยนชื่อแอนิเมชันเป็น GroundFire_idle)
        if (anim != null) anim.Play("GroundFire_idle");
        if (fireHitbox != null) fireHitbox.SetActive(false);

        // 5. รีเซ็ตระบบ รอรับการเหยียบครั้งต่อไป
        isActivated = false;
    }
}