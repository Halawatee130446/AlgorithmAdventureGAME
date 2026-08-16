using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ต้องใช้ตัวนี้เพื่อดึงเลขฉาก

public class CoinSpawner : MonoBehaviour
{
    public int targetTotalCoins = 80;

    public GameObject smallCoinPrefab;
    public int smallCoinValue = 1;

    public GameObject bigCoinPrefab;
    public int bigCoinValue = 3;

    public List<Transform> spawnPoints;

    void Start()
    {
        SpawnCoinsToMatchTarget();
    }

    private void SpawnCoinsToMatchTarget()
    {
        int currentSpawnedValue = 0;

        // 🟢 ล็อคผลการสุ่ม (Seed) ให้เหมือนเดิมทุกครั้งที่โหลดซีนด่านนี้ เหรียญจะได้ไม่ย้ายที่!
        Random.InitState(SceneManager.GetActiveScene().buildIndex);

        // สลับตำแหน่งแบบคงที่
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Transform temp = spawnPoints[i];
            int randomIndex = Random.Range(i, spawnPoints.Count);
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }

        foreach (Transform pt in spawnPoints)
        {
            if (currentSpawnedValue >= targetTotalCoins) break;

            int difference = targetTotalCoins - currentSpawnedValue;
            GameObject prefabToSpawn = null;
            int valueToAdd = 0;

            if (difference >= bigCoinValue)
            {
                bool spawnBigCoin = Random.value > 0.5f;
                if (spawnBigCoin)
                {
                    prefabToSpawn = bigCoinPrefab;
                    valueToAdd = bigCoinValue;
                }
                else
                {
                    prefabToSpawn = smallCoinPrefab;
                    valueToAdd = smallCoinValue;
                }
            }
            else if (difference >= smallCoinValue)
            {
                prefabToSpawn = smallCoinPrefab;
                valueToAdd = smallCoinValue;
            }

            if (prefabToSpawn != null)
            {
                // กินโควต้ายอดรวมไว้ก่อน
                currentSpawnedValue += valueToAdd;

                // 🟢 เช็คประวัติ! ถ้ายังไม่เคยเก็บ ค่อยเสกตัวตนของมันออกมา
                if (PlayerPrefs.GetInt("CoinCollected_" + pt.name, 0) == 0)
                {
                    GameObject coinObj = Instantiate(prefabToSpawn, pt.position, Quaternion.identity);

                    // แปะป้ายชื่อ (ID) ให้เหรียญ โดยใช้ชื่อของจุดเกิด (เช่น "SpawnPoint (5)")
                    Coin coinScript = coinObj.GetComponent<Coin>();
                    if (coinScript != null)
                    {
                        coinScript.coinID = pt.name;
                    }
                }
            }
        }

        // 🟢 คืนค่าระบบสุ่มให้เป็นอิสระตามเวลาจริง เพื่อไม่ให้กระทบกับระบบอื่นในเกม
        Random.InitState(System.Environment.TickCount);
    }
}