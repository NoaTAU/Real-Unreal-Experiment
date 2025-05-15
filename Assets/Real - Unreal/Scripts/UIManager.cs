using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameObject dialogGameObject;
    private Dialog dialog;

    private void Start()
    {
        dialog = new Dialog(dialogGameObject);
        //Test:
        dialog.ShowAndWaitForButtonPress("מאיה מאיה אחד שיפט אחד");
    }

}


public class Dialog
{
    private GameObject dialog;
    private Toggle confirmButton;
    private TextMeshProUGUI messageText;

    public Dialog(GameObject dialog)
    {
        this.dialog = dialog;
        confirmButton = dialog.GetComponentInChildren<Toggle>();
        messageText = dialog.transform.Find("BodyText").GetComponent<TextMeshProUGUI>();

        if (confirmButton == null)
        {
            LogHelper.Log("Confirm button not found in dialog.", "#ff33f3");
        }
        if (messageText == null)
        {
            LogHelper.Log("Message text not found in dialog.", "#ff33f3");
        }
        if (dialog == null)
        {
            LogHelper.Log("Dialog not found.", "#ff33f3");
        }
    }
    public void Show()
    {
        confirmButton.isOn = false;
        dialog.SetActive(true);
    }
    public void Hide()
    {
        dialog.SetActive(false);
    }
    public void ShowAndWaitForButtonPress()
    {
        Show();
        confirmButton.onValueChanged.AddListener(OnConfirmButtonPressed);
    }
    private void OnConfirmButtonPressed(bool isOn)
    {
        if (!isOn) return;
        Hide();
        TXRDataManager.Instance.LogLineToFile($"Confirm button pressed - {dialog.name}");
        confirmButton.onValueChanged.RemoveListener(OnConfirmButtonPressed);
    }
    public void ShowAndWaitForButtonPress(string message)
    {
        Show();
        confirmButton.onValueChanged.AddListener(OnConfirmButtonPressed);

        if (messageText != null)
        {
            messageText.text = message;
        }
    }
}