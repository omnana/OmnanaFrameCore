using UnityEngine;
using UnityEngine.UI;

public class ProfileWarningItem : MonoBehaviour
{
    public Text MsgText;
    
    public GameObject BigPanel;
    public GameObject SmallPanel;

    public Button BigPanelBtn;
    public Button SmallPanelBtn;
    
    void Awake()
    {
        BigPanelBtn.onClick.AddListener(OnBigTipClick);
        SmallPanelBtn.onClick.AddListener(OnSmallTipClick);
    }

    public void Open(string tip)
    {
        MsgText.text = tip;
        gameObject.SetActive(true);
        BigPanel.SetActive(!SmallPanel.activeSelf);
        transform.SetAsLastSibling();
    }

    private void OnBigTipClick()
    {
        BigPanel.SetActive(false);
        SmallPanel.SetActive(true);
    }

    private void OnSmallTipClick()
    {
        BigPanel.SetActive(true);
        SmallPanel.SetActive(false);
    }
}
