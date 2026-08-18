using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 🟢 1. เปลี่ยนชื่อ Class ให้ตรงกับชื่อไฟล์ใหม่ เพื่อไม่ให้ซ้ำกับระบบหลัก
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
    [SerializeField] private GameObject bookPage; 

    [Header("Quiz Point Settings")]
    [SerializeField] private GameObject[] questionMarkPoints;

    private PlayerController playerMovement;
    private PlayerShooting playerShooting;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (KnowledgePanel != null) KnowledgePanel.SetActive(false);

        if (PlayerPrefs.GetInt(chestID + "_isOpened", 0) == 1)
        {
            isOpened = true;
            anim.Play("Opened");
            anim.SetInteger("treasureState", 4);
        }

        if (PlayerPrefs.GetInt(chestID + "_hasRead", 0) == 1)
        {
            hasRead = true;
            ShowQuestionMarks();

            if (InGameKnowledgeBook.Instance != null && bookPage != null)
            {
                InGameKnowledgeBook.Instance.AddPage(bookPage);
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

        PlayerPrefs.SetInt(chestID + "_isOpened", 1);
        PlayerPrefs.Save();

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
            PlayerPrefs.SetInt(chestID + "_hasRead", 1);
            PlayerPrefs.Save();
            ShowQuestionMarks();

            if (InGameKnowledgeBook.Instance != null && bookPage != null)
            {
                InGameKnowledgeBook.Instance.AddPage(bookPage);
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