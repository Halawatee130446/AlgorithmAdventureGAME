using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionPoint : MonoBehaviour
{
    [Header("Scene Destination")]
    [SerializeField] private string sceneToLoad;

    [Header("UI Settings")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private UnityEngine.UI.Text hintText;

    private bool playerInside = false;

    // 1. เพิ่มตัวแปร Animator
    private Animator anim;

    private void Start()
    {
        // 2. ดึง Component Animator มาเก็บไว้ตอนเริ่มเกม
        anim = GetComponent<Animator>();

        if (notificationPanel != null) notificationPanel.SetActive(false);
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.S))
        {
            EnterQuizScene();
        }
    }

    private void EnterQuizScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // --- โค้ดที่เพิ่มเข้ามา: เซฟตำแหน่งปัจจุบันของกบเขียวก่อนย้าย Scene ---
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerPrefs.SetFloat("ReturnPosX", player.transform.position.x);
                PlayerPrefs.SetFloat("ReturnPosY", player.transform.position.y);
                PlayerPrefs.SetInt("IsReturningFromMiniGame", 1); // ตั้งค่าสถานะว่ากำลังจะกลับมา
                PlayerPrefs.Save();
            }
            // -------------------------------------------------------------

            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("ยังไม่ได้ใส่ชื่อ Scene ที่ต้องการให้ไป ในสคริปต์ QuestionPoint ครับ!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = true;

            // 3. สั่งให้แอนิเมชันเปลี่ยนไปเล่นท่า showS_Q
            if (anim != null) anim.SetBool("isPlayerNear", true);

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(true);
                if (hintText != null) hintText.text = "Press S to Enter Quiz!";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = false;

            // 4. สั่งให้แอนิเมชันกลับไปเล่นท่า idle_Q
            if (anim != null) anim.SetBool("isPlayerNear", false);

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
        }
    }
}