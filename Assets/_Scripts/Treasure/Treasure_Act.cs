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

    [Header("UI Settings")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Text hintText;

    [Header("Knowledge UI Settings")]
    [SerializeField] private GameObject KnowledgePanel;

    // --- แก้ไขเป็น Array ([]) เพื่อให้ใส่ได้หลายจุด ---
    [Header("Quiz Point Settings")]
    [SerializeField] private GameObject[] questionMarkPoints;
    // ----------------------------------------

    private PlayerController playerMovement;
    private PlayerShooting playerShooting;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (KnowledgePanel != null) KnowledgePanel.SetActive(false);

        // เช็คว่าเคยเปิดหรือยัง?
        if (PlayerPrefs.GetInt(chestID + "_isOpened", 0) == 1)
        {
            isOpened = true;

            // --- 1. บังคับเล่นท่า Opened ทันที (อ้างอิงชื่อจากรูป Animator ของคุณ) ---
            anim.Play("Opened");
            anim.SetInteger("treasureState", 4); // ตั้งค่า state เผื่อไว้ตอนผู้เล่นเดินเข้าออก

            // --- 2. เรียกใช้ฟังก์ชันโชว์จุดคำถาม ---
            ShowQuestionMarks();
        }
        else
        {
            // ถ้ายังไม่เคยเปิด ก็ซ่อนตามปกติ
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

        // เรียกใช้ฟังก์ชันโชว์จุดคำถาม
        ShowQuestionMarks();
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
    }

    public void CloseKnowledge()
    {
        Player_canMove();
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

    // --- 3. ฟังก์ชันใหม่: เอาไว้กรองว่าจุดไหนตอบถูกแล้วให้ซ่อน จุดไหนยังไม่ตอบให้โชว์ ---
    private void ShowQuestionMarks()
    {
        foreach (GameObject qp in questionMarkPoints)
        {
            if (qp != null)
            {
                // ดึงสคริปต์ QuestionPoint มาอ่านค่า ID
                QuestionPoint qScript = qp.GetComponent<QuestionPoint>();

                if (qScript != null)
                {
                    // เช็คว่าข้อนี้เคยตอบถูก (Passed) หรือยัง?
                    if (PlayerPrefs.GetInt(qScript.questionID + "_Passed", 0) == 0)
                    {
                        qp.SetActive(true); // ถ้ายังไม่ผ่าน ให้โชว์
                    }
                    else
                    {
                        qp.SetActive(false); // ถ้าผ่านแล้ว ให้ซ่อนถาวร
                    }
                }
                else
                {
                    qp.SetActive(true); // เผื่อลืมใส่สคริปต์ไว้
                }
            }
        }
    }
}

/* เก่าาาาาาาا

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure_Act : MonoBehaviour
{
    private Animator anim;

    private bool playerInside = false;
    private bool isOpened = false;

    [Header("UI Settings")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Text hintText;

    [Header("Knowledge UI Settings")]
    [SerializeField] private GameObject KnowledgePanel;

    private PlayerController playerMovement;
    private PlayerShooting playerShooting;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (KnowledgePanel != null) KnowledgePanel.SetActive(false);
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

            // ถ้าผู้เล่นเดินหนี (หลุดระยะหีบ) ให้ปิดหน้าต่างด้วย
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
    }

    // ฟังก์ชันรอรับสัญญาณสาธารณะ (public) เมื่อปุ่มกากบาทถูกกด
    public void CloseKnowledge()
    {
        Player_canMove();
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

    // เพิ่มฟังก์ชันนี้เพื่อให้สคริปต์อื่นมาถามได้ว่า "เปิดหีบหรือยัง?"
    public bool CheckIfOpened()
    {
        return isOpened;
    }
}
*/