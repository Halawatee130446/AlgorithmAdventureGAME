using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int baseHealth = 4;
    // ลบ public int maxHealth = 4; ออกไปได้เลยครับ จะได้ไม่ซ้ำซ้อนกัน
    public int currentHealth;

    [SerializeField] private HealthUI healthUI;
    [SerializeField] private bool useExtraHeartBuff = false;

    public bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 1. เช็คก่อนว่าเพิ่งกลับมาจากมินิเกมหรือเปล่า?
        if (GameManager.Instance != null && GameManager.Instance.isReturningFromMiniGame)
        {
            // ถ้าใช่ ให้ดึงเลือดเก่าที่ฝากไว้มาใช้เลย
            currentHealth = GameManager.Instance.savedHealth;
        }
        else
        {
            // 2. ถ้าไม่ใช่ (แปลว่าเริ่มด่านเล่นใหม่ปกติ) ค่อยคำนวณเลือดแบบปกติ
            if (useExtraHeartBuff)
            {
                currentHealth = baseHealth + 1;
            }
            else
            {
                currentHealth = baseHealth;
            }
        }

        // 3. เมื่อได้ค่า currentHealth ที่ถูกต้องแล้ว ค่อยสั่งอัปเดต UI รูปหัวใจ 
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
    }

    public bool TakeDamage(int damageAmount)
    {
        // ถ้าเป็นอมตะอยู่ ให้ข้ามการโดนดาเมจไปเลย
        if (isInvincible) return false;

        currentHealth -= damageAmount;
        Debug.Log("โดนโจมตี! หัวใจเหลือ: " + currentHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth <= 0)
        {
            return true; // เลือดหมด ส่งค่ากลับไปว่า ตายแล้ว
        }
        else
        {
            // เปิดโหมดอมตะทันที! เพื่อป้องกันการโดนตีซ้ำตอนโดนสต๊าฟ
            isInvincible = true;
            return false;
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกให้เริ่มวิบวับนับเวลา 1.5 วินาที
    public void StartFlashingAndUnlock()
    {
        StartCoroutine(FlashingRoutine());
    }

    private IEnumerator FlashingRoutine() // ฟังก์ชันนี้จะทำให้ตัวละครวิบวับเป็นเวลา 2 วินาที
    {
        float elapsedTime = 0f;

        // วิบวับเป็นเวลา 2 วินาที 
        while (elapsedTime < 2f)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.2f;
        }

        // ชัวร์ว่าสีกลับมาปกติ และปลดโหมดอมตะให้รับดาเมจครั้งต่อไปได้
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        isInvincible = false;
    }
}