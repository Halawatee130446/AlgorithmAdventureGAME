using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public GameObject collectEffect;

    // เพิ่มตัวแปรนี้เข้ามา
    public string coinID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(coinValue);
            }

            // 🟢 เซฟประวัติลงสมองเกมว่า เหรียญรหัสนี้ถูกเก็บไปแล้ว!
            if (!string.IsNullOrEmpty(coinID))
            {
                PlayerPrefs.SetInt("CoinCollected_" + coinID, 1);
                PlayerPrefs.Save();
            }

            CoinUIManager ui = Object.FindFirstObjectByType<CoinUIManager>();
            if (ui != null)
            {
                ui.UpdateCoinUI(); // สั่งให้อัปเดต UI ทันที
            }

            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}