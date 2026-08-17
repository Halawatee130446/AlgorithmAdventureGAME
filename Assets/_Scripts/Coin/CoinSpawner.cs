using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Random.InitState(SceneManager.GetActiveScene().buildIndex);

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
                currentSpawnedValue += valueToAdd;

                // 🟢 เช็คว่า GameManager จำได้ไหมว่าเหรียญนี้โดนเก็บไปแล้วในรอบนี้
                if (GameManager.Instance != null && GameManager.Instance.IsCoinCollected(pt.name))
                {
                    continue; // ถ้าเก็บไปแล้ว ให้ข้าม ไม่ต้องเสก!
                }

                GameObject coinObj = Instantiate(prefabToSpawn, pt.position, Quaternion.identity);
                Coin coinScript = coinObj.GetComponent<Coin>();
                if (coinScript != null)
                {
                    coinScript.coinID = pt.name;
                }
            }
        }

        Random.InitState(System.Environment.TickCount);
    }
}