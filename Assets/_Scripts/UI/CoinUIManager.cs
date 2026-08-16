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
            // ดึงเงินจากตู้เซฟหลักมาโชว์เลย จะได้ไม่โดน UI หลอกอีก!
            coinText.text = "x " + GameManager.Instance.globalCoins.ToString();
        }
    }
}