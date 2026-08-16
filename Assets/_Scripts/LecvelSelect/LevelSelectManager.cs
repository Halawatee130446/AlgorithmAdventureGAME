using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [Header("ชื่อซีนที่ต้องการไป")]
    public string mainMenuScene = "MainMenu";
    public string level1Scene = "Level1";
    public string level2Scene = "Level2";
    public string level3Scene = "Level3";

    [Header("ปุ่มเข้าด่าน และ ไอคอนล็อค")]
    public Button[] levelButtons; // ลากปุ่มด่าน 1, 2, 3 มาใส่ตามลำดับ
    public GameObject[] lockIcons; // ลากภาพแม่กุญแจของด่าน 1, 2, 3 มาใส่ (ด่าน 1 อาจจะปล่อยว่างไว้ถ้าไม่มีล็อค)

    [Header("ระบบไอเทมเสริม (Boosters)")]
    public Text itemStatusText; // Text บนปุ่มไอเทมเพื่อบอกว่า "ใช้งานอยู่" หรือ "พร้อมใช้"

    // ตัวแปรซ่อนไว้ใช้คำนวณเบื้องหลัง
    private int availableHearts = 0;
    private bool isUsingExtraHeart = false; // สถานะว่ากดเปิดใช้ไอเทมหรือยัง

    void Start()
    {
        // 1. เช็คด่านที่ปลดล็อค (ถ้ายังไม่เคยเล่นเลย จะให้ค่าเริ่มต้นเป็น 1)
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                // ถ้าด่านนั้นสูงกว่าระดับที่เล่นถึง -> ล็อคปุ่ม และโชว์กุญแจ
                levelButtons[i].interactable = false;
                if (lockIcons.Length > i && lockIcons[i] != null) lockIcons[i].SetActive(true);
            }
            else
            {
                // ถ้าด่านนั้นเล่นถึงแล้ว -> ปลดล็อคปุ่ม และซ่อนกุญแจ
                levelButtons[i].interactable = true;
                if (lockIcons.Length > i && lockIcons[i] != null) lockIcons[i].SetActive(false);
            }
        }

        // 2. เช็คจำนวนไอเทมเสริมที่มี (สมมติว่าเซฟชื่อ Item_ExtraHeart มาจาก Knowledge Library)
        availableHearts = PlayerPrefs.GetInt("Item_ExtraHeart", 0);
        UpdateItemUI();
    }

    // --- ส่วนของปุ่มย้อนกลับและเลือกด่าน ---
    public void ClickBackToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLevel(int levelIndex)
    {
        // 🟢 ก่อนย้ายซีน ให้ส่งข้อมูลไปบอก GameManager ว่าเราเปิดใช้ไอเทมเสริมหรือไม่
        if (GameManager.Instance != null)
        {
            // เดี๋ยวเราค่อยไปเพิ่มตัวแปร isUsingExtraHeart ใน GameManager ทีหลัง
            // GameManager.Instance.isUsingExtraHeart = isUsingExtraHeart; 
        }

        // 🟢 ถ้ากดยืนยันใช้ไอเทม ให้หักยอดในกระเป๋าด้วย
        if (isUsingExtraHeart)
        {
            availableHearts--;
            PlayerPrefs.SetInt("Item_ExtraHeart", availableHearts);
            PlayerPrefs.Save();
        }

        // โหลดซีนตามตัวเลขด่าน
        if (levelIndex == 1) SceneManager.LoadScene(level1Scene);
        else if (levelIndex == 2) SceneManager.LoadScene(level2Scene);
        else if (levelIndex == 3) SceneManager.LoadScene(level3Scene);
    }

    // --- ส่วนของปุ่มไอเทมเสริม ---
    public void ClickToggleItem()
    {
        if (availableHearts > 0)
        {
            isUsingExtraHeart = !isUsingExtraHeart; // สลับสถานะ เปิด <-> ปิด
            UpdateItemUI();
        }
        else
        {
            Debug.Log("ไม่มีไอเทมเหลืออยู่! ต้องไปตอบควิซใน Knowledge Library ก่อน");
        }
    }

    private void UpdateItemUI()
    {
        if (itemStatusText != null)
        {
            if (availableHearts <= 0)
            {
                itemStatusText.text = "ไม่มีไอเทม";
                itemStatusText.color = Color.gray;
            }
            else if (isUsingExtraHeart)
            {
                itemStatusText.text = "ใช้งานอยู่ (" + availableHearts + ")";
                itemStatusText.color = Color.green; // เปลี่ยนตัวหนังสือเป็นสีเขียวเพื่อให้รู้ว่าเปิดใช้แล้ว
            }
            else
            {
                itemStatusText.text = "ใช้ +1 (" + availableHearts + ")";
                itemStatusText.color = Color.white;
            }
        }
    }
}