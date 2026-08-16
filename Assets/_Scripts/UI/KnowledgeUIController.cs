using UnityEngine;

public class KnowledgeUIController : MonoBehaviour
{
    public GameObject knowledgePanel; // ลากหน้าต่าง Panel ที่ต้องการปิดมาใส่

    public void ClosePanelAndUnlockPlayer()
    {
        // 1. ปิดหน้าต่าง UI
        if (knowledgePanel != null)
        {
            knowledgePanel.SetActive(false);
        }

        // 2. ปลดฟรีซ คืนเวลาให้เกมกลับมาเดินปกติ
        Time.timeScale = 1f;

        // 3. ค้นหาตัวกบเขียว (Player) แล้วเปิดสคริปต์ให้กลับมาเดินและยิงได้
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = true;

            PlayerShooting ps = player.GetComponent<PlayerShooting>();
            if (ps != null) ps.enabled = true;
        }
    }
}