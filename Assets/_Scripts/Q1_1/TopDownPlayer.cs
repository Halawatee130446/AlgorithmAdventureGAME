using UnityEngine;

public class TopDownPlayer : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // รับค่าปุ่มลูกศร (แนวนอน และ แนวตั้ง)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // สั่งให้เดิน (normalized เพื่อไม่ให้เดินเฉียงเร็วเกินไป)
        rb.velocity = new Vector2(h, v).normalized * speed;
    }
}