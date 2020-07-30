using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
    public Text errorMessage;
    public Button confirm;
    public GameObject dialogPanelObject;

    private static DialogPanel dialog;

    public static DialogPanel Instance ()
    {
        if (!dialog)
        {
            dialog = FindObjectOfType<DialogPanel>() as DialogPanel;
            if(!dialog)
            {
                Debug.LogError("There must be one active DialogPanel script on a GameObject in the scene.");
            }
        }
        return dialog;
    }

    public void ShowPanel (string message)
    {
        dialogPanelObject.SetActive(true);

        confirm.onClick.RemoveAllListeners();
        confirm.onClick.AddListener(ClosePanel);

        this.errorMessage.text = message;
        confirm.gameObject.SetActive(true);

    }

    void ClosePanel()
    {
        dialogPanelObject.SetActive(false);
    }
}
