using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientInfoPanel : MonoBehaviour,IPanel
{
    public Text caseNumberText;
    public InputField firstName, lastName;
    public GameObject locationPanel;

    public void OnEnable()
    {
        caseNumberText.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
    }

    public void ProcessInfo()
    {
        //Check if firstname && lastname are not empty
        if (string.IsNullOrEmpty(firstName.text) || string.IsNullOrEmpty(lastName.text))
        {
            Debug.Log("Enter the first of last name is empty and we can't continue");            
        }

        else
        {
            UIManager.Instance.activeCase.name = firstName.text + " " + lastName.text;
            locationPanel.SetActive(true);
        }
    }
}
