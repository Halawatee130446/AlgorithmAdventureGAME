using System.Collections;
using UnityEngine;

public class SpringMechanism : MonoBehaviour
{
    [Header("ตั้งค่าความแรงของสปริง")]
    public float bounceForce = 15f; // ปรับตัวเลขนี้เพื่อให้เด้งสูงขึ้นหรือต่ำลง
    public float animationResetTime = 0.5f; // เวลาที่สปริงค้างท่าเด้ง ก่อนจะหดกลับไปท่าเดิม

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // บังคับให้เล่นท่า Idle ตอนเริ่มเกม
        if (anim != null) anim.Play("Idle");
    }

    // เมื่อมีวัตถุ (แบบของแข็ง) มาชน
    void OnCollisionEnter2D(Collision2D collision)
    {
        // เช็คว่าเป็นกบเขียว (Player) ไหม
        if (collision.gameObject.CompareTag("Player"))
        {
            // เช็คว่ากบเขียวอยู่ "สูงกว่า" ฐานสปริง (เพื่อบังคับว่าต้องกระโดดเหยียบจากด้านบนเท่านั้น)
            if (collision.transform.position.y > transform.position.y)
            {
                BouncePlayer(collision.gameObject);
            }
        }
    }

    private void BouncePlayer(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 1. เคลียร์แรงตกเดิมทิ้งก่อน (เพื่อให้กบเด้งได้ความสูงเท่าเดิมเสมอ ไม่ว่าจะตกมาจากที่สูงแค่ไหน)
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            // 2. ออกแรงผลักกบเขียวพุ่งขึ้นไปตรงๆ
            rb.velocity = new Vector2(rb.velocity.x, bounceForce);

            // 3. เล่นแอนิเมชันสปริงเด้ง (ชื่อ Work ตามภาพของคุณ)
            if (anim != null) anim.Play("Work");

            // 4. สั่งหน่วงเวลาให้สปริงหดกลับไปท่า Idle ตามเดิม
            StopAllCoroutines(); // กันบั๊กเหยียบซ้ำรัวๆ
            StartCoroutine(ResetAnimationRoutine());
        }
    }

    private IEnumerator ResetAnimationRoutine()
    {
        // รอเวลาแป๊บนึงให้แอนิเมชันท่า Work เล่นจนจบ
        yield return new WaitForSeconds(animationResetTime);

        // สั่งกลับไปเล่นท่า Idle รอรับการเหยียบครั้งต่อไป
        if (anim != null) anim.Play("Idle");
    }
}