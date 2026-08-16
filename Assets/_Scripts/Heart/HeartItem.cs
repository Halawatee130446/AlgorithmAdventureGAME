using UnityEngine;

public class HeartItem : MonoBehaviour
{
    [Header("ตั้งชื่อ ID ให้หัวใจแต่ละดวงห้ามซ้ำกัน!")]
    public string itemID; // เช่น "Heart_Map1_1", "Heart_SecretRoom"

    public int healValue = 1;

    private void Start()
    {
        // 1. ตอนเริ่มด่าน ให้มันเช็คก่อนว่า หัวใจ ID นี้ เคยถูกเก็บไปหรือยัง?
        // (1 = เคยเก็บแล้ว, 0 = ยังไม่เคยเก็บ)
        if (PlayerPrefs.GetInt(itemID, 0) == 1)
        {
            // ถ้าเคยเก็บไปแล้ว ให้ทำลายตัวเองทิ้งตั้งแต่แวบแรกที่โหลดด่านเลย!
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem playerHealth = collision.GetComponent<HealthSystem>();

            if (playerHealth != null)
            {
                if (playerHealth.currentHealth < playerHealth.currentMaxHealth)
                {
                    playerHealth.Heal(healValue);

                    // 2. พอเก็บปุ๊บ ให้จดบันทึกลงสมุดของเกมว่า "ไอเทม ID นี้โดนเก็บแล้วนะ"
                    PlayerPrefs.SetInt(itemID, 1);
                    PlayerPrefs.Save(); // เซฟทับทันที

                    Destroy(gameObject);
                }
            }
        }
    }
}