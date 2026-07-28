using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int baseHealth = 4;
    private int currentHealth;

    [SerializeField] private HealthUI healthUI;
    [SerializeField] private bool useExtraHeartBuff = false;

    public bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (useExtraHeartBuff)
        {
            currentHealth = baseHealth + 1;
        }
        else
        {
            currentHealth = baseHealth;
        }

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

