using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureAction : MonoBehaviour
{
    private Animator anim;
    
    private bool playerInside = false; // ตรวจความพร้อมว่าตัวละครอยู่ในระยะหีบไหม
    private bool isOpened = false;     // ตรวจว่าหีบนี้เคยถูกเปิดไปแล้วหรือยัง

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // ถ้าตัวละครอยู่ในบริเวณหีบ
        if (playerInside)
        {
            // เงื่อนไขที่ 1: หีบยังไม่เคยเปิด และผู้เล่นกดปุ่ม O
            if (!isOpened && Input.GetKeyDown(KeyCode.O))
            {
                OpenTreasure();
            }

            // เงื่อนไขที่ 2: ถ้าหีบเปิดแล้ว และผู้เล่นกดปุ่ม R ให้ไปทำฟังก์ชันอ่านคลังความรู้
            if (isOpened && Input.GetKeyDown(KeyCode.R))
            {
                ReadKnowledge();
            }
        }
    }

    // ฟังก์ชันสั่งเปิดหีบสมบัติ
    private void OpenTreasure()
    {
        isOpened = true;
        anim.SetInteger("treasureState", 2); // เล่นอนิเมชั่น TreasureOpen
        
        // (เมื่อเอนิเมชั่น TreasureOpen เล่นจบ มันจะเปลี่ยนไปท่า pressR เองอัตโนมัติด้วย Has Exit Time ใน Animator)
    }

    // ตรวจสอบเมื่อตัวละครเดินเข้ามาในพื้นที่หีบสมบัติ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = true;

            if (!isOpened)
            {
                // ถ้ายังไม่เคยเปิดหีบ -> ขึ้นอนิเมชั่นกด O ทันที
                anim.SetInteger("treasureState", 1); // เล่นอนิเมชั่น preessO
            }
            else
            {
                // ถ้าเคยเปิดหีบไปแล้ว -> เดินเข้ามาใหม่ให้ขึ้นอนิเมชั่นกด R ทันที
                anim.SetInteger("treasureState", 3); // เล่นอนิเมชั่น pressR
            }
        }
    }

    // ตรวจสอบเมื่อตัวละครเดินออกจากพื้นที่หีบสมบัติ
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = false;

            if (!isOpened)
            {
                // เดินออกไปโดยยังไม่เปิด ให้หีบกลับไปสถานะปิดเฉยๆ (0)
                anim.SetInteger("treasureState", 0);
            }
            else
            {
                // เดินออกไปทั้งที่เปิดแล้ว ให้ซ่อนปุ่มกด R โดยเปลี่ยนไปเป็นท่าหีบเปิดค้างไว้เฉยๆ (4)
                anim.SetInteger("treasureState", 4); // เล่นอนิเมชั่น Opened
            }
        }
    }

    // ฟังก์ชันสำหรับเชื่อมโยงไปเปิด UI คลังความรู้
    private void ReadKnowledge()
    {
        Debug.Log("Player pressed R: Opening Knowledge Interface!");
        // โค้ดสำหรับสั่งเปิด UI คลังความรู้ของคุณฮาลาวาตี สามารถใส่ตรงนี้ได้เลยครับ[cite: 1]
    }
}