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

        // 🟢 1. เช็คก่อนว่าด่านนี้ใช้ "ไอเทมเสริม" ไหม? (เพื่อกำหนดหลอดเลือด Max Health)
        bool useItem = useExtraHeartBuff;
        if (GameManager.Instance != null && GameManager.Instance.isUsingExtraHeart)
        {
            useItem = true;
        }

        if (useItem)
        {
            currentMaxHealth = absoluteMaxHealth; // ขยายหลอดเลือดรอไว้เลยเป็น 6
        }
        else
        {
            currentMaxHealth = 5;                 // หลอดปกติเก็บได้ 5
        }

        // 🟢 2. กำหนดเลือดปัจจุบัน (Current Health) ให้กบเขียว
        if (GameManager.Instance != null && GameManager.Instance.isReturningFromMiniGame)
        {
            // ถ้ากลับมาจากมินิเกม ให้ดึงเลือดที่ฝากไว้มาใส่
            currentHealth = GameManager.Instance.savedHealth;
        }
        else
        {
            // ถ้าเป็นการเข้าด่านครั้งแรก
            if (useItem)
            {
                currentHealth = baseHealth + 1; // เริ่มที่ 5 ดวง
            }
            else
            {
                currentHealth = baseHealth;     // เริ่มที่ 4 ดวง
            }
        }

        // ป้องกันไม่ให้เลือดล้นหลอด
        if (currentHealth > currentMaxHealth)
        {
            currentHealth = currentMaxHealth;
        }

        // 3. อัปเดต UI รูปหัวใจ 
        UpdateHealthUI();
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > currentMaxHealth)
        {
            currentHealth = currentMaxHealth;
        }

        UpdateHealthUI();
        Debug.Log("เก็บหัวใจ! เลือดตอนนี้: " + currentHealth + "/" + currentMaxHealth);
    }

    public void UpgradeMaxHealth()
    {
        if (currentMaxHealth < absoluteMaxHealth)
        {
            currentMaxHealth++;
            currentHealth = currentMaxHealth;
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
    }

    public bool TakeDamage(int damageAmount)
    {
        if (isInvincible) return false;

        currentHealth -= damageAmount;
        Debug.Log("โดนโจมตี! หัวใจเหลือ: " + currentHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            return true;
        }
        else
        {
            isInvincible = true;
            return false;
        }
    }

    public void StartFlashingAndUnlock()
    {
        StartCoroutine(FlashingRoutine());
    }

    private IEnumerator FlashingRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);

            elapsedTime += 0.2f;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        isInvincible = false;
    }
}