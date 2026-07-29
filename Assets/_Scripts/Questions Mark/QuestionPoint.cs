using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionPoint : MonoBehaviour
{
    [Header("Scene Destination")]
    [SerializeField] private string sceneToLoad;
    [Header("Save Settings")]
    public string questionID = "Q1_1"; // ต้องตั้งให้ตรงกับ questionID ใน MiniGameManager ของด่านนั้นๆ นะครับ

    [Header("UI Settings")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private UnityEngine.UI.Text hintText;

    // --- เพิ่มบรรทัดนี้เข้ามา ---


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
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                GameManager.Instance.returnPosition = player.transform.position;

                HealthSystem hs = player.GetComponent<HealthSystem>();
                if (hs != null) GameManager.Instance.savedHealth = hs.currentHealth;

                // --- เพิ่มการฝากกระสุนตรงนี้ ---
                PlayerShooting ps = player.GetComponent<PlayerShooting>();
                if (ps != null) GameManager.Instance.savedAmmo = ps.currentAmmo;

                GameManager.Instance.isReturningFromMiniGame = true;
            }
            SceneManager.LoadScene(sceneToLoad);
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