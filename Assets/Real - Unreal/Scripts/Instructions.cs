
using UnityEngine;
using UnityEngine.UI;



public class Instructions : MonoBehaviour
{

    private Toggle confirmButton;


    private void Start()
    {
        confirmButton = GetComponentInChildren<Toggle>();
        //Only for testing purposes:
        ShowAndWaitForButtonPress();

    }

    private void Show()
    {
        confirmButton.isOn = false;
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
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
        TXRDataManager.Instance.LogLineToFile($"Confirm button pressed - {gameObject.name}");
        confirmButton.onValueChanged.RemoveListener(OnConfirmButtonPressed);
    }


}

