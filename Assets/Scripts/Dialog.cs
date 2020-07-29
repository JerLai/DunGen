using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Text errorMessage;
    public Button confirm;
    public GameObject dialogPanel;

    private static Dialog dialog;

    public static Dialog Instance ()
    {
        if (!dialog)
        {
            dialog = FindObjectOfType<Dialog>() as Dialog;
            if(!dialog)
            {
                Debug.LogError("There must be one active Dialog script on a GameObject in the scene.");
            }
        }
        return dialog;
    }


}
