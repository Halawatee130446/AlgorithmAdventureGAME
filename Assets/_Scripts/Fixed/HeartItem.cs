using UnityEngine;

public class HeartItem : MonoBehaviour
{
    public string itemID;
    public int healValue = 1;

    private void Start()
    {
        // 🟢 ใช้ฟังก์ชันเช็คไอเทมจาก SaveManager
        if (SaveManager.IsItemCollected(itemID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem playerHealth = collision.GetComponent<HealthSystem>();

            if (playerHealth != null && playerHealth.currentHealth < playerHealth.currentMaxHealth)
            {
                playerHealth.Heal(healValue);

                // 🟢 บันทึกการเก็บไอเทมผ่าน SaveManager
                SaveManager.SetItemCollected(itemID);

                Destroy(gameObject);
            }
        }
    }
}