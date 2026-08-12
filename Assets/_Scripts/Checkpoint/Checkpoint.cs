using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    private bool isActivated = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        // บังคับให้เล่นท่าตอนยังไม่เช็คอิน (เผื่อกันเหนียว)
        if (anim != null) anim.Play("idle_Checkpoint");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้ากบเขียวเดินมาชน (Trigger) และจุดนี้ยังไม่เคยถูกเปิดใช้งาน
        if (collision.CompareTag("Player") && !isActivated)
        {
            isActivated = true; // ล็อกไว้ จะได้ไม่ทำงานซ้ำเวลาเดินผ่านอีกรอบ

            // 1. สั่งให้เล่นแอนิเมชันจุดเช็คอินทำงาน
            if (anim != null) anim.Play("Checked_anim");

            // 2. บันทึกข้อมูลลงสมอง GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.hasCheckpoint = true;

                // เซฟพิกัดของเสา Checkpoint ต้นนี้เอาไว้
                GameManager.Instance.lastCheckpointPos = transform.position;

                Debug.Log("เช็คอินเรียบร้อย! เซฟตำแหน่งเกิดใหม่ไว้ที่: " + transform.position);
            }
        }
    }
}