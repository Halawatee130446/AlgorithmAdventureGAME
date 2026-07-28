using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoving : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWaypointIndex = 0;
    [SerializeField] private float speed = 2f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // 1. เพิ่มตัวแปร Rigidbody2D
    private Rigidbody2D rb;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // ดึง Component มาใช้
    }

    // 2. เปลี่ยนจาก Update เป็น FixedUpdate (จำเป็นมากเมื่อใช้คำสั่งเกี่ยวกับฟิสิกส์)
    private void FixedUpdate()
    {
        if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < .1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }

        Vector2 targetPosition = waypoints[currentWaypointIndex].transform.position;
        Vector2 direction = targetPosition - (Vector2)transform.position;

        if (direction.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = false;
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }

        // 3. ขยับด้วยระบบฟิสิกส์ของ Rigidbody2D แทน transform.position
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
}

/* เก่าาาาา
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoving : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWaypointIndex = 0;

    [SerializeField] private float speed = 2f;

    // 1. เพิ่มตัวแปรสำหรับจัดการภาพและแอนิเมชัน
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Start()
    {
        // 2. ดึง Component ในตัวมอนสเตอร์มาเก็บไว้ในตัวแปรตอนเริ่มเกม
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // เช็คระยะห่างว่าเดินมาถึงจุด waypoint ปัจจุบันหรือยัง
        if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < .1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }

        // 3. หาว่าเป้าหมายต่อไปอยู่ทางซ้ายหรือขวา เพื่อหันหน้าให้ถูกฝั่ง
        Vector2 targetPosition = waypoints[currentWaypointIndex].transform.position;
        Vector2 direction = targetPosition - (Vector2)transform.position;

        if (direction.x > 0)
        {
            // ถ้าจุดหมายอยู่ทางขวา  Flip ภาพ (ตัวละครจะหันไปทางซ้ายตามปกติ)
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            // ถ้าจุดหมายอยู่ทางซ้าย ไม่ต้อง Flip ภาพแกน X (ตัวละครจะหันไปทางขาว)
            spriteRenderer.flipX = false;
        }

        // 4. สั่งให้ Animator เล่นแอนิเมชันเดิน
        // เราส่งค่า true ไปยัง Parameter ที่ชื่อว่า "isMoving" ใน Animator
        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }

        // สั่งให้เคลื่อนที่ไปยังจุดหมาย
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed);
    }
}

*/