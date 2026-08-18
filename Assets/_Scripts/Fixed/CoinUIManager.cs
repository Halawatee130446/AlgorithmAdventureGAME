using UnityEngine;
using UnityEngine.UI;

public class CoinUIManager : MonoBehaviour
{
    public static CoinUIManager Instance; // 🟢 ทำเป็น Singleton
    public Text coinText;

    void Awake()
    {
        // กำหนดตัวมันเองเป็น Instance กลาง
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        if (coinText != null && GameManager.Instance != null)
        {
            coinText.text = "x " + GameManager.Instance.sessionCoins.ToString();
        }
    }
}