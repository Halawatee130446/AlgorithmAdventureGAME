using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure_Act : MonoBehaviour
{
    [Header("Save Settings")]
    public string chestID = "Chest_1";
    private Animator anim;

    private bool playerInside = false;
    private bool isOpened = false;
    private bool hasRead = false; // 🟢 เพิ่มตัวแปรเช็คว่าเคยอ่านหรือยัง

    [Header("Knowledge UI Settings")]

    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Text hintText;

    [Header("Knowledge UI Settings")]
    [SerializeField] private GameObject KnowledgePanel;
    [SerializeField] private GameObject bookPage; // 🟢 เพิ่มอันนี้ สำหรับหน้ากระดาษที่จะส่งเข้าสมุดรวม!

    [Header("Quiz Point Settings")]
    [SerializeField] private GameObject[] questionMarkPoints;

    private PlayerController playerMovement;
    private PlayerShooting playerShooting;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (KnowledgePanel != null) KnowledgePanel.SetActive(false);

        // 1. เช็คว่าเคยเปิดหรือยัง?
        if (PlayerPrefs.GetInt(chestID + "_isOpened", 0) == 1)
        {
            isOpened = true;
            anim.Play("Opened");
            anim.SetInteger("treasureState", 4);
        }

        // 🟢 2. เช็คว่าเคยอ่านหนังสือหรือยัง? (ถ้าเคยอ่านแล้วถึงจะโชว์คำถาม)
        if (PlayerPrefs.GetInt(chestID + "_hasRead", 0) == 1)
        {
            hasRead = true;
            ShowQuestionMarks();

            // 🟢 ถ้าโหลดฉากมาแล้วพบว่าเคยอ่านหีบนี้แล้ว ให้ส่งหน้ากระดาษเข้าสมุดรวมไปเลย
            if (InGameKnowledgeBook.Instance != null && bookPage != null)
            {
                InGameKnowledgeBook.Instance.AddPage(bookPage);
            }
        }
        else
        {
            // ถ้ายังไม่เคยอ่าน ให้ซ่อนจุดคำถามไว้ก่อน
            foreach (GameObject qp in questionMarkPoints)
            {
                if (qp != null) qp.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (playerInside)
        {
            if (!isOpened && Input.GetKeyDown(KeyCode.O))
            {
                OpenTreasure();
            }

            if (isOpened && Input.GetKeyDown(KeyCode.R))
            {
                ReadKnowledge();
            }
        }
    }

    private void OpenTreasure()
    {
        isOpened = true;
        anim.SetInteger("treasureState", 3);

        PlayerPrefs.SetInt(chestID + "_isOpened", 1);
        PlayerPrefs.Save();

        if (hintText != null)
        {
            hintText.text = "Press R to Read!";
        }

        // 🟢 เอา ShowQuestionMarks(); ออกจากตรงนี้ เพราะเปิดกล่องอย่างเดียวยังไม่ให้ทำควิซ
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = true;
            playerMovement = collision.gameObject.GetComponent<PlayerController>();
            playerShooting = collision.gameObject.GetComponent<PlayerShooting>();

            if (notificationPanel != null) notificationPanel.SetActive(true);

            if (!isOpened)
            {
                anim.SetInteger("treasureState", 1);
                if (hintText != null) hintText.text = "Press O to Open!";
            }
            else
            {
                anim.SetInteger("treasureState", 3);
                if (hintText != null) hintText.text = "Press R to Read!";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = false;

            if (notificationPanel != null) notificationPanel.SetActive(false);

            if (KnowledgePanel != null) KnowledgePanel.SetActive(false);
            Player_canMove();

            if (!isOpened) anim.SetInteger("treasureState", 0);
            else anim.SetInteger("treasureState", 4);
        }
    }

    private void ReadKnowledge()
    {
        if (KnowledgePanel != null) KnowledgePanel.SetActive(true);
        if (notificationPanel != null) notificationPanel.SetActive(false);
        Player_cantMove();

        // 🟢 หยุดเวลาในเกม เพื่อให้อ่านได้อย่างสบายใจ
        Time.timeScale = 0f;

        // 🟢 ถ้าเพิ่งเคยกดอ่านครั้งแรก ให้เซฟประวัติและเสกจุดคำถามออกมา!
        if (!hasRead)
        {
            hasRead = true;
            PlayerPrefs.SetInt(chestID + "_hasRead", 1);
            PlayerPrefs.Save();
            ShowQuestionMarks();

            // 🟢 ถ้าเพิ่งกดอ่านครั้งแรก ก็ให้ส่งหน้ากระดาษเข้าสมุดรวมแบบสดๆ ร้อนๆ!
            if (InGameKnowledgeBook.Instance != null && bookPage != null)
            {
                InGameKnowledgeBook.Instance.AddPage(bookPage);
            }
        }
    }

    public void CloseKnowledge()
    {
        Player_canMove();

        // 🟢 คืนเวลาให้เกมเดินต่อ เมื่อกดปิดหน้าต่าง
        Time.timeScale = 1f;
    }

    private void Player_cantMove()
    {
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerShooting != null) playerShooting.enabled = false;
    }

    private void Player_canMove()
    {
        if (playerMovement != null) playerMovement.enabled = true;
        if (playerShooting != null) playerShooting.enabled = true;
    }

    private void ShowQuestionMarks()
    {
        foreach (GameObject qp in questionMarkPoints)
        {
            if (qp != null)
            {
                QuestionPoint qScript = qp.GetComponent<QuestionPoint>();

                if (qScript != null)
                {
                    if (PlayerPrefs.GetInt(qScript.questionID + "_Passed", 0) == 0)
                    {
                        qp.SetActive(true);
                    }
                    else
                    {
                        qp.SetActive(false);
                    }
                }
                else
                {
                    qp.SetActive(true);
                }
            }
        }
    }
}