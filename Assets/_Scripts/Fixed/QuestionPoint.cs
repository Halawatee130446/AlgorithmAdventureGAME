using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionPoint : MonoBehaviour
{
    [Header("Scene Destination")]
    [SerializeField] private string sceneToLoad;

    [Header("Save Settings")]
    public string questionID = "Q1_1";

    [Header("UI Settings")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private UnityEngine.UI.Text hintText;

    private bool playerInside = false;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (notificationPanel != null) notificationPanel.SetActive(false);
    }

    private void Update()
    {
        // ถ้ายืนอยู่ในจุด และกดปุ่ม S
        if (playerInside && Input.GetKeyDown(KeyCode.S))
        {
            EnterQuizScene();
        }
    }

    private void EnterQuizScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // 🟢 โยนหน้าที่การเซฟของทั้งหมดไปให้ GameManager ทำคำสั่งเดียวจบ!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SavePlayerStateBeforeMiniGame();
            }

            // โหลดหน้ามินิเกม
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = true;
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
            if (anim != null) anim.SetBool("isPlayerNear", false);

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
        }
    }
}