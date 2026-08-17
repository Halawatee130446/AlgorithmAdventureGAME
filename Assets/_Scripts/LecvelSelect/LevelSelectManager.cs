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
    public Button[] levelButtons;
    public GameObject[] lockIcons;

    [Header("ระบบไอเทมเสริม (Boosters)")]
    public Text itemStatusText;

    private int availableHearts = 0;
    private bool isUsingExtraHeart = false;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isUsingExtraHeart = false;
        }

        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                levelButtons[i].interactable = false;
                if (lockIcons.Length > i && lockIcons[i] != null) lockIcons[i].SetActive(true);
            }
            else
            {
                levelButtons[i].interactable = true;
                if (lockIcons.Length > i && lockIcons[i] != null) lockIcons[i].SetActive(false);
            }
        }

        availableHearts = PlayerPrefs.GetInt("Item_ExtraHeart", 0);
        UpdateItemUI();
    }

    public void ClickBackToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLevel(int levelIndex)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isUsingExtraHeart = isUsingExtraHeart;

            // 🟢 ไม้ตายแก้บั๊กตายทิพย์! บังคับให้เกมรู้ว่า "นี่คือการเข้าด่านใหม่สดๆ ห้ามดึงค่าเวลา/เลือดที่เป็น 0 มาใช้เด็ดขาด!"
            GameManager.Instance.isReturningFromMiniGame = false;
        }

        if (isUsingExtraHeart)
        {
            availableHearts--;
            PlayerPrefs.SetInt("Item_ExtraHeart", availableHearts);
            PlayerPrefs.Save();
        }

        if (levelIndex == 1) SceneManager.LoadScene(level1Scene);
        else if (levelIndex == 2) SceneManager.LoadScene(level2Scene);
        else if (levelIndex == 3) SceneManager.LoadScene(level3Scene);
    }

    public void ClickToggleItem()
    {
        if (availableHearts > 0)
        {
            isUsingExtraHeart = !isUsingExtraHeart;
            UpdateItemUI();
        }
        else
        {
            Debug.Log("No items left!");
        }
    }

    public void ClickGetFreeHeartTest()
    {
        availableHearts += 5;
        PlayerPrefs.SetInt("Item_ExtraHeart", availableHearts);
        PlayerPrefs.Save();
        UpdateItemUI();
        Debug.Log("ได้หัวใจฟรี 5 ดวงสำหรับการทดสอบ!");
    }

    private void UpdateItemUI()
    {
        if (itemStatusText != null)
        {
            if (availableHearts <= 0)
            {
                itemStatusText.text = "Empty (" + availableHearts + ")";
                itemStatusText.color = Color.gray;
            }
            else if (isUsingExtraHeart)
            {
                itemStatusText.text = "Equipped (" + availableHearts + ")";
                itemStatusText.color = new Color(0f, 0.5f, 0f);
            }
            else
            {
                itemStatusText.text = "Available (" + availableHearts + ")";
                itemStatusText.color = Color.black;
            }
        }
    }
}