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

    // 🟢 1. เพิ่มตัวแปรล็อคสถานะ เพื่อไม่ให้ตายซ้ำซ้อน
    private bool isAlreadyDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isAlreadyDead || healthSystem.isInvincible) return;

        if (collision.gameObject.CompareTag("Trap"))
        {
            bool isDead = healthSystem.TakeDamage(1);
            if (isDead)
            {
                Die();
            }
            else
            {
                // 🟢 เปิดให้เล่นอนิเมชัน Hurt ตอนโดนกับดัก!
                anim.SetTrigger("hurt");
                StartCoroutine(RespawnAfterDelay());
            }
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            bool isStomping = false;

            foreach (ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y > 0.3f)
                {
                    isStomping = true;
                    break;
                }
            }

            if (transform.position.y > collision.transform.position.y + 0.3f)
            {
                isStomping = true;
            }

            if (isStomping)
            {
                rb.velocity = new Vector2(rb.velocity.x, 12f);
                MonsterHealth monsterHealth = collision.gameObject.GetComponent<MonsterHealth>();
                if (monsterHealth != null)
                {
                    monsterHealth.TakeDamage();
                }
            }
            else
            {
                bool isDead = healthSystem.TakeDamage(1);
                if (isDead)
                {
                    Die();
                }
                else
                {
                    // 🟢 มอนสเตอร์ ปิดไว้เหมือนเดิม (ไม่ต้องเล่นอนิเมชัน)
                    // anim.SetTrigger("hurt"); 

                    Knockback(collision.transform);
                    healthSystem.StartFlashingAndUnlock();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isAlreadyDead || healthSystem.isInvincible) return;

        if (collision.gameObject.CompareTag("Trap"))
        {
            bool isDead = healthSystem.TakeDamage(1);
            if (isDead)
            {
                Die();
            }
            else
            {
                // 🟢 เปิดให้เล่นอนิเมชัน Hurt ตอนโดนกับดักแบบทะลุ (เช่น กองไฟ)
                anim.SetTrigger("hurt");
                StartCoroutine(RespawnAfterDelay());
            }
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.canMove = false;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(0.8f);

        // 🟢 [ไม้ตายสลายบั๊ก] เคลียร์สถานะ Animator ก่อนวาร์ป!
        // บังคับให้ลืมท่า Hurt และกลับไปเป็นท่า Idle (ค่า state = 0)
        anim.ResetTrigger("hurt");
        anim.SetInteger("state", 0);

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

        if (GameManager.Instance != null && GameManager.Instance.hasCheckpoint)
        {
            transform.position = GameManager.Instance.lastCheckpointPos;
        }
        else if (nearestSpot != null)
        {
            transform.position = nearestSpot.position;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        if (pc != null) pc.canMove = true;

        healthSystem.StartFlashingAndUnlock();
    }

    private void Knockback(Transform damageSource)
    {
        Vector2 knockbackDirection = (transform.position - damageSource.position).normalized;
        knockbackDirection.y = 0.5f;

        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        GetComponent<PlayerController>().KnockbackLock(0.7f);
    }

    private void Die()
    {
        if (isAlreadyDead) return;
        isAlreadyDead = true;

        StopAllCoroutines();

        // 🟢 เปลี่ยนมาเรียกคำสั่งตายจากที่เดียว ไม่ต้องตั้งค่า Rigidbody/Animator เองแล้ว
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.Die();
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