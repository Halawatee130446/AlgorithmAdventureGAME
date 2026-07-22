using UnityEngine;
using UnityEngine.UI;

public class NextButtonHandler : MonoBehaviour
{
    [SerializeField] private KnowledgeUIManager uiManager; // ลากวัตถุ Knowledge มาใส่

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null && uiManager != null)
        {
            btn.onClick.AddListener(uiManager.GoToNextPage);
        }
    }
}