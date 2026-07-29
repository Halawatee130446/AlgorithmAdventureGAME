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
}

/* เวอร์เก่าตอนยังไม่เพิ่มการกระเด็น

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour // สคริปต์ควบคุมการเคลื่อนที่ การกระโดด และ Animation ของผู้เล่น
{
    private Rigidbody2D rb; //ประกาศตัวแปร rb มีชนิดเป็น Rigidbody2D (ใช้ควบคุมการเคลื่อนที่ด้วยระบบฟิสิกส์)
    private BoxCollider2D coll; //ประกาศตัวแปร coll มีชนิดเป็น BoxCollider2D (ใช้ตรวจสอบการชนและการยืนบนพื้น)
    private SpriteRenderer sprite; //ประกาศตัว sprite มีชนิดเป็น SpriteRenderer (ใช้พลิกตัวละครซ้าย-ขวา)
    private Animator anim; //ประกาศตัว anim มีชนิดเป็น Animator
    [SerializeField] private LayerMask jumpbleGround; //สร้างช่องสำหรับเลือกเลเยอร์(ไว้ใช้สำหรับการกระโดด)
    private float dirX = 0f; //ประกาศตัว dirX ใช้เก็บค่าการเดิน (-1 = ซ้าย, 0 = หยุด, 1 = ขวา)
    [SerializeField] private float moveSpeed = 7f; //ประกาศตัว moveSpeed ใช้เก็บค่าความเร็วในการเดิน (กำหนดเป็น7 แต่มีช่องแก้หน้างานได้)
    [SerializeField] private float jumpForce = 9.5f; // แรงกระโดด
    private enum MovementState { idle, running, jumping, falling }     // สถานะ Animation ของตัวละคร ( 0 , 1 , 2 , 3 ) ตามลำดับค่าใน condition – state ที่เรากำหนด
    [SerializeField] private AudioSource jumpSoundEffect;  // ช่องเสียงเอฟเฟกต์ตอนกระโดด


    private void Start() // เรียกครั้งเดียวเมื่อเริ่มเกม ใช้เรียกค่าต่างๆใน Unity มาใส่ในตัวแปรที่เราประกาศ
    {
        // ดึง Component ต่าง ๆ ของ GameObject มาเก็บไว้ใช้งาน
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        Debug.Log(gameObject.name); //เขียนให้แสดงวัตถุเฉยๆ
        Debug.Log(rb.bodyType); //แสดง bodyType ของ Rigibody2D
    }


    private void Update() // เรียกทุกเฟรมของเกม เกมจะเล่นส่วนนี้แบบเฟรมต่อเฟรม คอยสังเกตว่ารับค่ามามั้ยตลอกเวลา อะไรประมาณนั้น
    {
        dirX = Input.GetAxisRaw("Horizontal"); //1.อ่านค่าการกดปุ่มเก็บไว้ในตัวแปร x (จะได้ค่า -1, 0, หรือ 1)
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y); // 2.กำหนดความเร็วในการเดิน (แกน X)  แกน Y ใช้ค่าเดิมเพื่อไม่ให้กระทบการกระโดด

        if (Input.GetButtonDown("Jump") && IsGrounded()) // ถ้ากดกระโดดและกำลังยืนบนพื้น
        {
            //jumpSoundEffect.Play(); // เล่นเสียงกระโดด ****คอมเม้นไว้ก่อนเพราะยังไม่ได้ใส่เสียง*****
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // เพิ่มแรงกระโดดในแกน Y เพื่อทำการกระโดด
        }

        UpdateAnimationState(); // อัปเดต Animation ตามสถานะการเคลื่อนที่
    }

    private void UpdateAnimationState()     // เปลี่ยน Animation ให้ตรงกับการเคลื่อนที่
    {
        MovementState state; // ตัวแปรใช้เก็บสถานะ Animation ปัจจุบัน
       
        if (dirX > 0f) // กรณีเดินไปทางขวา
        {
            state = MovementState.running; //กำหนดค่า state ให้เรียกใช้ท่าทางการวิ่ง   
            sprite.flipX = false;   // หันหน้าขวา
        }
        
        else if (dirX < 0f) // เดินไปทางซ้าย
        {
            state = MovementState.running; //กำหนดค่า state ให้เรียกใช้ท่าทางการวิ่ง   
            sprite.flipX = true;    // พลิก Sprite ให้หันซ้าย
        }
        
        else // ไม่เคลื่อนที่
        {
            state = MovementState.idle; //กำหนดค่า state ให้เรียกใช้ท่าทางตอนยืนเฉยๆ state = 0
        }

        // ถ้าลอยขึ้น ให้เล่น Animation กระโดด
        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        // ถ้ากำลังตก ให้เล่น Animation ตก
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        // ส่งค่าของสถานะไปยัง Animator
        anim.SetInteger("state", (int)state);
    }

    // ตรวจสอบว่าผู้เล่นกำลังยืนอยู่บนพื้นหรือไม่
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(
            coll.bounds.center,   // จุดเริ่มต้นของการตรวจสอบ
            coll.bounds.size,     // ขนาดของกล่องตรวจสอบ
            0f,                   // ไม่หมุนกล่อง
            Vector2.down,         // ตรวจลงด้านล่าง
            .1f,                  // ระยะตรวจสอบ
            jumpbleGround         // ตรวจเฉพาะ Layer พื้น
        );
    }
}

*/