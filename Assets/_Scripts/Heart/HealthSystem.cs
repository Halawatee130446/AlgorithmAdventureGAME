using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int baseHealth = 4;         // เลือดเริ่มต้นตอนเข้าด่าน
    public int currentMaxHealth = 5;   // ขีดจำกัดเลือดที่เก็บได้ในด่านนี้ (5)
    public int absoluteMaxHealth = 6;  // ขีดจำกัดสูงสุดเผื่ออัปเกรดในอนาคต (6)

    public int currentHealth;

    [Header("System References")]
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

        // ป้องกันไม่ให้ตอนเริ่มเกม เลือดเกินขีดจำกัด
        if (currentHealth > currentMaxHealth)
        {
            currentHealth = currentMaxHealth;
        }

        // 3. อัปเดต UI รูปหัวใจ 
        UpdateHealthUI();
    }

    // --- 🟢 ฟังก์ชันใหม่: สำหรับเก็บไอเทมหัวใจตามด่าน ---
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        // ถ้าฮีลแล้วเลือดทะลุหลอดลิมิตปัจจุบัน (เช่น ทะลุ 5) ให้โดนตัดกลับมาเท่าลิมิต
        if (currentHealth > currentMaxHealth)
        {
            currentHealth = currentMaxHealth;
        }

        UpdateHealthUI();
        Debug.Log("เก็บหัวใจ! เลือดตอนนี้: " + currentHealth + "/" + currentMaxHealth);
    }

    // --- 🌟 ฟังก์ชันใหม่เผื่ออนาคต: อัปเกรดหลอดเลือดเป็น 6 ดวง ---
    public void UpgradeMaxHealth()
    {
        if (currentMaxHealth < absoluteMaxHealth)
        {
            currentMaxHealth++; // ขยายหลอดเลือดจาก 5 เป็น 6
            currentHealth = currentMaxHealth; // เติมเลือดให้เต็มหลอดใหม่ด้วย
            UpdateHealthUI();
            Debug.Log("อัปเกรดเลือดสูงสุดเป็น: " + currentMaxHealth);
        }
    }

    // ฟังก์ชันตัวกลางสำหรับสั่งอัปเดต UI (จะได้ไม่ต้องเขียนซ้ำหลายรอบ)
    private void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
    }

    // --- โค้ดเดิมของคุณที่ทำงานได้ดีเยี่ยมอยู่แล้ว ---
    public bool TakeDamage(int damageAmount)
    {
        // ถ้าเป็นอมตะอยู่ ให้ข้ามการโดนดาเมจไปเลย
        if (isInvincible) return false;

        currentHealth -= damageAmount;
        Debug.Log("โดนโจมตี! หัวใจเหลือ: " + currentHealth);

        UpdateHealthUI();

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

    private IEnumerator FlashingRoutine()
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