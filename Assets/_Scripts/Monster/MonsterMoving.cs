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