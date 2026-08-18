using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureBoxController : MonoBehaviour
{
    [Header("Save Settings")]
    public string chestID = "Chest_1";
    private Animator anim;

    private bool playerInside = false;
    private bool isOpened = false;
    private bool hasRead = false;

    [Header("UI Settings")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Text hintText;
    [SerializeField] private GameObject KnowledgePanel;

    // 🟢 ลบช่อง bookPage เก่าทิ้งไปแล้วนะครับ ไม่ต้องลากใส่แล้ว!

    [Header("Quiz Point Settings")]
    [SerializeField] private GameObject[] questionMarkPoints;

    private PlayerController playerMovement;
    private PlayerShooting playerShooting;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (KnowledgePanel != null) KnowledgePanel.SetActive(false);

        if (SaveManager.IsChestOpened(chestID))
        {
            isOpened = true;
            anim.Play("Opened");
            anim.SetInteger("treasureState", 4);
        }

        if (SaveManager.IsChestRead(chestID))
        {
            hasRead = true;
            ShowQuestionMarks();

            // 🟢 ตอนเปิดเกมมาเช็คว่าเคยอ่านหีบนี้แล้ว ให้โยนรูปเข้าสมุดไว้เลย
            if (InGameKnowledgeBook.Instance != null && KnowledgePanel != null)
            {
                KnowledgeUIManager uiManager = KnowledgePanel.GetComponent<KnowledgeUIManager>();
                if (uiManager != null && uiManager.knowledgePages != null)
                {
                    InGameKnowledgeBook.Instance.AddPages(uiManager.knowledgePages);
                }
            }
        }
        else
        {
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
        SaveManager.SetChestOpened(chestID);

        if (hintText != null)
        {
            hintText.text = "Press R to Read!";
        }
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

        Time.timeScale = 0f;

        if (!hasRead)
        {
            hasRead = true;
            SaveManager.SetChestRead(chestID);
            ShowQuestionMarks();

            // 🟢 ตอนอ่านหีบครั้งแรก ให้กวาดรูปจากหีบ ส่งเข้าสมุดรวมทั้งหมดเลย
            if (InGameKnowledgeBook.Instance != null && KnowledgePanel != null)
            {
                KnowledgeUIManager uiManager = KnowledgePanel.GetComponent<KnowledgeUIManager>();
                if (uiManager != null && uiManager.knowledgePages != null)
                {
                    InGameKnowledgeBook.Instance.AddPages(uiManager.knowledgePages);
                }
            }
        }
    }

    public void CloseKnowledge()
    {
        Player_canMove();
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
                    qp.SetActive(!SaveManager.IsMiniGamePassed(qScript.questionID));
                }
                else
                {
                    qp.SetActive(true);
                }
            }
        }
    }
}