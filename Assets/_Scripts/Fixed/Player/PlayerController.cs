using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    [SerializeField] private LayerMask jumpbleGround;
    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 9.5f;
    private enum MovementState { idle, running, jumping, falling }
    [SerializeField] private AudioSource jumpSoundEffect;

    // 1. เพิ่มตัวแปรเช็คว่าตอนนี้สามารถเดินได้ไหม
    public bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // --- โค้ดที่เพิ่มเข้ามา: เช็คว่าเพิ่งกลับมาจากมินิเกมหรือเปล่า ---
        if (PlayerPrefs.GetInt("IsReturningFromMiniGame", 0) == 1)
        {
            float returnX = PlayerPrefs.GetFloat("ReturnPosX");
            float returnY = PlayerPrefs.GetFloat("ReturnPosY");
            transform.position = new Vector2(returnX, returnY); // วาร์ปกบไปตำแหน่งเดิม

            // ล้างค่าสถานะทิ้ง จะได้ไม่วาร์ปมารันตอนเริ่มเกมครั้งหน้า
            PlayerPrefs.SetInt("IsReturningFromMiniGame", 0);
            PlayerPrefs.Save();
        }

        if (GameManager.Instance != null && GameManager.Instance.isReturningFromMiniGame)
        {
            // ดึงพิกัดที่ฝากไว้กลับมาใช้งาน
            transform.position = GameManager.Instance.returnPosition;
        }
    }

    private void Update()
    {
        // 2. ถ้าขยับไม่ได้ (กำลังกระเด็น) ให้ข้ามคำสั่งเกี่ยวกับการควบคุมด้านล่างไปเลย
        if (!canMove) return;

        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            //jumpSoundEffect.Play(); 
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpbleGround);
    }

    // ---------------------------------------------------
    // 3. เพิ่มฟังก์ชันสำหรับรับแรงกระเด็น
    // ---------------------------------------------------
    // เปลี่ยนให้ฟังก์ชันรับค่าเวลา (duration) ได้
    public void KnockbackLock(float duration)
    {
        StartCoroutine(KnockbackRoutine(duration));
    }

    private IEnumerator KnockbackRoutine(float duration)
    {
        canMove = false; // ปลดการควบคุม (ห้ามขยับ)
        yield return new WaitForSeconds(duration); // รอเวลาตามที่ส่งมา (ให้เท่ากับแอนิเมชันเจ็บ)
        canMove = true; // คืนการควบคุม
    }

    // ---------------------------------------------------
    // 🟢 โค้ดที่เพิ่มใหม่: ศูนย์กลางการสั่งตาย
    // ---------------------------------------------------
    public void Die()
    {
        canMove = false;
        this.enabled = false; // ปิดสคริปต์ตัวเองไม่ให้เดินต่อ

        // บังคับหยุดฟิสิกส์
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // เล่นท่าตาย
        if (anim != null)
        {
            anim.SetTrigger("death");
        }

        // รอ 0.8 วินาทีแล้วโชว์หน้าต่าง Game Over
        Invoke("CallGameOver", 0.8f);
    }

    private void CallGameOver()
    {
        GameOverManager gameOverSystem = Object.FindFirstObjectByType<GameOverManager>();
        if (gameOverSystem != null)
        {
            gameOverSystem.ShowGameOver();
        }
    }
}

