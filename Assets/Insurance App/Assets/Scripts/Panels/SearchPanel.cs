using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchPanel : MonoBehaviour, IPanel
{
    public InputField caseNumberInput;

    public void ProcessInfo()
    {
        //Download list of all objects inside s3 storage (serviceappcasefileshong)

        AWSManager.Instance.GetList(caseNumberInput.text);

        //compare those to caseNumberInput by user

        //if we find a match
        //Download that object
    }
}
