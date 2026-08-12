using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trap_Death : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private HealthSystem healthSystem;

    [SerializeField] private float knockbackForce = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (healthSystem.isInvincible) return;

        if (collision.gameObject.CompareTag("Trap"))
        {
            bool isDead = healthSystem.TakeDamage(1);
            if (isDead)
            {
                Die();
            }
            else
            {
                anim.SetTrigger("hurt");
                StartCoroutine(RespawnAfterDelay());
            }
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            // -----------------------------------------------------------------
            // ระบบเช็คการเหยียบ (Stomp Detection) ที่แม่นยำขึ้น
            // -----------------------------------------------------------------
            bool isStomping = false;

            // 1. วนลูปเช็คจุดสัมผัส "ทุกจุด" (แก้บั๊กโดนมุมแล้วตีความผิด)
            foreach (ContactPoint2D point in collision.contacts)
            {
                // ลดเกณฑ์จาก 0.5 เหลือ 0.3 เพื่อให้เหยียบเฉียงๆ ก็ยังนับว่าโดนหัว
                if (point.normal.y > 0.3f)
                {
                    isStomping = true;
                    break;
                }
            }

            // 2. กันเหนียวเพิ่ม: ถ้าจุดศูนย์กลางกบเขียว อยู่สูงกว่ามอนสเตอร์ ก็ให้ถือว่าเหยียบหัวแน่ๆ
            if (transform.position.y > collision.transform.position.y + 0.3f)
            {
                isStomping = true;
            }

            // -----------------------------------------------------------------

            if (isStomping)
            {
                // 1. ใส่แรงกระโดดให้กบเขียวเด้งขึ้น 
                rb.velocity = new Vector2(rb.velocity.x, 12f);

                // 2. เรียกสคริปต์ลดเลือดมอนสเตอร์
                MonsterHealth monsterHealth = collision.gameObject.GetComponent<MonsterHealth>();
                if (monsterHealth != null)
                {
                    monsterHealth.TakeDamage();
                }
            }
            else // ถ้าไม่ได้ชนจากด้านบน (ชนด้านข้าง/ล่าง) กบเขียวจะเจ็บตามปกติ
            {
                bool isDead = healthSystem.TakeDamage(1);
                if (isDead)
                {
                    Die();
                }
                else
                {
                    anim.SetTrigger("hurt");
                    Knockback(collision.transform);
                    healthSystem.StartFlashingAndUnlock();
                }
            }
        }
    }

    // เพิ่มฟังก์ชันนี้เพื่อให้กบเขียวรับดาเมจจากสิ่งของทะลุได้ (Trigger) เช่น เปลวไฟ
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (healthSystem.isInvincible) return;

        if (collision.gameObject.CompareTag("Trap"))
        {
            bool isDead = healthSystem.TakeDamage(1);
            if (isDead)
            {
                Die();
            }
            else
            {
                anim.SetTrigger("hurt");
                StartCoroutine(RespawnAfterDelay());
            }
        }
    }


    private IEnumerator RespawnAfterDelay()
    {
        // 1. สต๊าฟตัวละคร: ล็อคเหมือนตาย ห้ามขยับ และหยุดฟิสิกส์
        GetComponent<PlayerController>().canMove = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // 2. รอเวลา 0.8 วินาที ให้เห็นท่า hurt ค้างไว้ (คุณปรับเพิ่มลดเวลาตรงนี้ได้ครับ)
        yield return new WaitForSeconds(0.8f);

        // 3. เริ่มหาจุดที่ใกล้ที่สุด
        GameObject[] safeSpots = GameObject.FindGameObjectsWithTag("SafeSpot");
        Transform nearestSpot = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject spot in safeSpots)
        {
            float distance = Vector2.Distance(transform.position, spot.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSpot = spot.transform;
            }
        }

        // 4. วาร์ปตัวละคร (ระบบใหม่: เช็ค Checkpoint ก่อน)
        if (GameManager.Instance != null && GameManager.Instance.hasCheckpoint)
        {
            // ถ้าเคยเหยียบ Checkpoint แล้ว ให้วาร์ปไปที่ Checkpoint ล่าสุด
            transform.position = GameManager.Instance.lastCheckpointPos;
            Debug.Log("วาร์ปกลับไปที่ Checkpoint!");
        }
        else if (nearestSpot != null)
        {
            // (ระบบเก่า) ถ้ายังไม่เคยเจอ Checkpoint เลย ค่อยใช้ SafeSpot ใกล้สุดแก้ขัด
            transform.position = nearestSpot.position;
            Debug.Log("ยังไม่มี Checkpoint... วาร์ปไป SafeSpot แทน");
        }

        // 5. ปลดสต๊าฟ: คืนระบบฟิสิกส์และสิทธิ์การควบคุมให้ผู้เล่นอีกครั้ง
        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<PlayerController>().canMove = true;

        // 6. วาร์ปเสร็จแล้ว ค่อยเริ่มกระพริบวิบวับป้องกันตัว 2 วินาที!
        healthSystem.StartFlashingAndUnlock();
    }

    private void Knockback(Transform damageSource)
    {
        Vector2 knockbackDirection = (transform.position - damageSource.position).normalized;
        knockbackDirection.y = 0.5f;

        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // ส่งเวลาไป 0.7f (ปรับตัวเลขนี้ให้เท่ากับความยาวแอนิเมชัน PlayerHurt ของคุณ)
        // เพื่อให้ตัวละครห้ามขยับจนกว่าแอนิเมชันจะเล่นจบ
        GetComponent<PlayerController>().KnockbackLock(0.7f);
    }

    private void Die()
    {
        GetComponent<PlayerController>().enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");

        // รอ 0.8 วิ แล้วเรียกหน้าต่าง Game Over (เหมือนกับตอนเวลาหมด)
        Invoke("TriggerGameOver", 0.8f);
    }

    private void TriggerGameOver()
    {
        GameOverManager gameOverSystem = Object.FindFirstObjectByType<GameOverManager>();
        if (gameOverSystem != null)
        {
            gameOverSystem.ShowGameOver();
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetStateOnDeath();
        }
    }
}

