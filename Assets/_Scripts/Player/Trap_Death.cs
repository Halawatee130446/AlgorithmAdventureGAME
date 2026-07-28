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
            bool isDead = healthSystem.TakeDamage(1);
            if (isDead)
            {
                Die();
            }
            else
            {
                anim.SetTrigger("hurt");
                Knockback(collision.transform);

                // ชนมอนสเตอร์ กระเด็นเสร็จให้เริ่มวิบวับทันที 1.5 วินาที
                healthSystem.StartFlashingAndUnlock();
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

        // 4. วาร์ปตัวละคร
        if (nearestSpot != null)
        {
            transform.position = nearestSpot.position;
        }

        // 5. ปลดสต๊าฟ: คืนระบบฟิสิกส์และสิทธิ์การควบคุมให้ผู้เล่นอีกครั้ง
        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<PlayerController>().canMove = true;

        // 6. วาร์ปเสร็จแล้ว ค่อยเริ่มกระพริบวิบวับป้องกันตัว 1.5 วินาที!
        healthSystem.StartFlashingAndUnlock();
    }

    private void Knockback(Transform damageSource)
    {
        Vector2 knockbackDirection = (transform.position - damageSource.position).normalized;
        knockbackDirection.y = 0.5f;

        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        GetComponent<PlayerController>().KnockbackLock();
    }

    private void Die()
    {
        GetComponent<PlayerController>().enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

