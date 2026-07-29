using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 2;
    private int currentHealth;

    private bool isDead = false;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D coll;

    private MonsterMoving movementScript;

    private void Start()
    {
        // ตั้งชื่อมอนสเตอร์แต่ละตัวให้ไม่ซ้ำกันใน Inspector (เช่น Mob1, Mob2)
        if (PlayerPrefs.GetInt(gameObject.name + "_Dead", 0) == 1)
        {
            Destroy(gameObject); // ถ้าเคยตาย ก็ลบทิ้งไปเลยตั้งแต่เริ่มซีน
            return;
        }

        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        movementScript = GetComponent<MonsterMoving>();
    }

    public void TakeDamage()
    {
        if (isDead || isInvincible) return;

        currentHealth--;
        Debug.Log("Monster Health : " + currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            // สั่งหยุดการทำงานของ Coroutine สตั๊นที่อาจจะกำลังค้างอยู่ทันที
            StopAllCoroutines();
            StartCoroutine(DieRoutine());
        }
        else
        {
            StartCoroutine(HitAndStunRoutine());
        }
    }

    private IEnumerator HitAndStunRoutine()
    {
        // 1. เปิดโหมดอมตะเพื่อบล็อกการโดนดาเมจเบิ้ลจาก OnCollisionStay2D (แค่ในเฟรมแรกๆ)
        isInvincible = true;

        if (movementScript != null) movementScript.enabled = false;

        spriteRenderer.color = Color.red;

        // รอแค่ 0.2 วินาที ให้พ้นจังหวะเหยียบเบิ้ล
        yield return new WaitForSeconds(0.2f);

        // 2. ปลดโหมดอมตะทันที! เพื่อให้ผู้เล่นสามารถโดดเหยียบซ้ำรอบสองได้ในขณะที่มันยังสตั๊นอยู่
        isInvincible = false;

        // 3. มอนสเตอร์เข้าสู่สถานะสตั๊น (ตัวยังขยับไม่ได้ และกระพริบเหลือง) 1.8 วินาที
        float stunTimer = 0f;
        while (stunTimer < 1.8f)
        {
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            stunTimer += 0.2f;
        }

        spriteRenderer.color = Color.white;

        // หมดเวลาสตั๊น สั่งให้กลับมาเดินต่อ
        if (movementScript != null) movementScript.enabled = true;
    }

    private IEnumerator DieRoutine()
    {
        isInvincible = true;
        coll.enabled = false;

        if (movementScript != null) movementScript.enabled = false;

        float elapsedTime = 0f;
        while (elapsedTime < 1.2f) // วิบวับเป็นเวลา 1.2 วินาที --> แล้วตายไปเลย
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = new Color(1f, 0f, 0f, 0.3f);
            yield return new WaitForSeconds(0.1f);

            elapsedTime += 0.2f;
        }

        Destroy(gameObject);
    }
}