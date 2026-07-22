using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton — ทำให้วัตถุอื่นเรียกใช้ GameManager ได้จากทุกที่
    public static GameManager Instance;

    // ตัวแปรเก็บข้อมูลหลักของเกม
    public int coins = 0;
    public int currentHP = 3;
    public int maxHP = 3;
    public float timeRemaining = 120f; // เวลา 2 นาทีต่อด่าน

    void Awake()
    {
        // ตั้งค่า Singleton
        Instance = this;
    }
}