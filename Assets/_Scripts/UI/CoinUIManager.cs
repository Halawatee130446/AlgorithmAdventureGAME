using UnityEngine;
using UnityEngine.UI;

public class CoinUIManager : MonoBehaviour
{
    public Text coinText;

    void Start()
    {
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        if (coinText != null && GameManager.Instance != null)
        {
            // 🟢 ดึงเฉพาะเงินในกระเป๋า (sessionCoins) มาโชว์ 
            // เพราะเงินในตู้เซฟ (globalCoins) เราจะเอาไปโชว์ใน Knowledge Library ตามที่คุณต้องการ
            coinText.text = "x " + GameManager.Instance.sessionCoins.ToString();
        }
    }
}