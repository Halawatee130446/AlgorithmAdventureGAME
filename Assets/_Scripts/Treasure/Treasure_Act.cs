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
    }

    private void Player_canMove()
    {
        if (playerMovement != null) playerMovement.enabled = true;
    }
}