using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ต้องมีบรรทัดนี้เพื่อคุยกับ UI Text

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // ลาก Bullet Prefab มาใส่
    [SerializeField] private Transform firePoint; // ลาก FirePoint มาใส่

    [SerializeField] private int maxAmmo = 15; // กระสุนเริ่มต้น
    public int currentAmmo;

    [SerializeField] private Text ammoText; // ลาก UI AmmoText มาใส่

    private SpriteRenderer playerSprite; // เอาไว้เช็คว่ากบหันซ้ายหรือขวา

    private void Start()
    {

        playerSprite = GetComponent<SpriteRenderer>();

        if (GameManager.Instance != null && GameManager.Instance.isReturningFromMiniGame)
        {
            currentAmmo = GameManager.Instance.savedAmmo; // ดึงกระสุนเดิมมา
        }
        else
        {
            currentAmmo = maxAmmo; // เริ่มเกมใหม่ กระสุนเต็ม
        }

        UpdateAmmoUI();
    }

    private void Update()
    {
        // ถ้ากดปุ่ม Q และกระสุนยังเหลือมากกว่า 0
        if (Input.GetKeyDown(KeyCode.Q) && currentAmmo > 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // 1. ลบกระสุน 1 นัด และอัปเดต UI
        currentAmmo--;
        UpdateAmmoUI();

        // 2. เสกกระสุนออกมาที่ตำแหน่ง FirePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        // 3. เช็คว่ากบเขียวหันหน้าไปทางไหน แล้วสั่งกระสุนให้พุ่งไปทางนั้น
        if (playerSprite.flipX == true)
        {
            // หันซ้าย (แกน X ติดลบ)
            bulletScript.SetDirection(-1f);
        }
        else
        {
            // หันขวา (แกน X เป็นบวก)
            bulletScript.SetDirection(1f);
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "X " + currentAmmo;
        }
    }
}