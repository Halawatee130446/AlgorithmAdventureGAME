using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    
    // สร้างตัวแปรไว้เก็บความสูงเริ่มต้นของกล้อง
    private float initialCameraY;

    private void Start()
    {
        // บันทึกความสูงของกล้อง ณ ตำแหน่งที่เราวางไว้ในหน้าต่าง Scene ตอนเริ่มเกม
        initialCameraY = transform.position.y;
    }

    private void Update()
    {
        // แกน X: ขยับตามตัวละครไปทางขวา 3 หน่วยเหมือนเดิม
        float newX = player.position.x + 3f; 
        
        // แกน Y: ใช้ความสูงเริ่มต้นตลอดเวลา ตัวละครจะโดดสูงแค่ไหน กล้องก็จะไม่ขยับขึ้น-ลง
        float newY = initialCameraY; 
        
        // แกน Z: ใช้ค่าเดิมของกล้อง
        float cameraZ = transform.position.z;
        
        transform.position = new Vector3(newX, newY, cameraZ);
    }
}