using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public GameObject collectEffect;
    public string coinID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. จับผิด GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(coinValue, coinID);
                Debug.Log("💰 GameManager ได้รับเงิน! ยอดรวมในกระเป๋าตอนนี้คือ: " + GameManager.Instance.sessionCoins);
            }
            else
            {
                Debug.LogError("🚨 พังจุดที่ 1: หา GameManager ไม่เจอ! ตัวแปร Instance เป็น null");
            }

            // 2. จับผิด CoinUIManager
            if (CoinUIManager.Instance != null)
            {
                CoinUIManager.Instance.UpdateCoinUI();
                Debug.Log("🖥️ ส่งคำสั่งให้อัปเดต UI หน้าจอแล้ว!");
            }
            else
            {
                Debug.LogError("🚨 พังจุดที่ 2: หา CoinUIManager ไม่เจอ! ลืมใส่สคริปต์ในฉากนี้หรือเปล่า?");
            }

            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}