using UnityEngine;
using UnityEngine.UI;

public class CloseButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject panelToClose;

    void Start()
    {
        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            btn.onClick.AddListener(OnClickClose);
        }
    }

    public void OnClickClose()
    {
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
            
        }
    }
}