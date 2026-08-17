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
            if (GameManager.Instance != null)
            {
                // 🟢 โยนเงินและรหัสเหรียญให้ GameManager เก็บเข้ากระเป๋าชั่วคราว
                GameManager.Instance.AddCoins(coinValue, coinID);
            }

            CoinUIManager ui = Object.FindFirstObjectByType<CoinUIManager>();
            if (ui != null)
            {
                ui.UpdateCoinUI();
            }

            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}