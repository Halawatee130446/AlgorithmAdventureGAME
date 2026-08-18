using UnityEngine;
using UnityEngine.UI;

public class KnowledgeUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image knowledgeImageDisplay;
    [SerializeField] private Text pageTextDisplay;

    [Header("Button Objects to Hide/Show")]
    [SerializeField] private GameObject nextButtonObject;
    [SerializeField] private GameObject previousButtonObject;

    [Header("Knowledge Contents")]
    // 🟢 แก้บรรทัดนี้เป็น public เพื่อให้สมุดไอคอนเข้ามาก็อปปี้รูปไปโชว์ได้
    public Sprite[] knowledgePages;

    private int currentPageIndex = 0;

    void OnEnable()
    {
        currentPageIndex = 0;
        UpdatePageUI();
    }

    public void GoToNextPage()
    {
        if (knowledgePages == null || knowledgePages.Length == 0) return;
        if (currentPageIndex >= knowledgePages.Length - 1) return;

        currentPageIndex++;
        UpdatePageUI();
    }

    public void GoToPreviousPage()
    {
        if (knowledgePages == null || knowledgePages.Length == 0) return;
        if (currentPageIndex <= 0) return;

        currentPageIndex--;
        UpdatePageUI();
    }

    private void UpdatePageUI()
    {
        if (knowledgePages == null || knowledgePages.Length == 0) return;

        if (knowledgeImageDisplay != null)
        {
            knowledgeImageDisplay.sprite = knowledgePages[currentPageIndex];
        }

        if (pageTextDisplay != null)
        {
            pageTextDisplay.text = (currentPageIndex + 1) + "/" + knowledgePages.Length;
        }

        ManageButtonVisibility();
    }

    private void ManageButtonVisibility()
    {
        if (previousButtonObject != null)
        {
            previousButtonObject.SetActive(currentPageIndex != 0);
        }

        if (nextButtonObject != null)
        {
            nextButtonObject.SetActive(currentPageIndex != knowledgePages.Length - 1);
        }
    }
}