using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The UI Manager is NULL");
            }

            return _instance;
        }
    }

    public Case activeCase;
    public ClientInfoPanel clientInfoPanel;
    public GameObject borderPanel;
    public GameObject AWSManagerPrefab;    

    private void Awake()
    {
        _instance = this;

        if (GameObject.Find("AWS_Manager") == null)
        {
            Instantiate(AWSManagerPrefab);
        }
    }

    public void CreateNewCase()
    {
        activeCase = new Case();

        //Generate a caseID
        //between 0 and 999
        //assign it to the active case
        int randomCaseID = Random.Range(0, 1000);

        activeCase.caseID = randomCaseID.ToString();

        clientInfoPanel.gameObject.SetActive(true);
        borderPanel.SetActive(true);
    }

    public void SubmitButton()
    {
        //create a new case to save
        //populate the case data
        //open a data stream to turn that object (Case) into a file
        //begin aws process

        Case awsCase = new Case();
        awsCase.caseID = activeCase.caseID;
        awsCase.name = activeCase.name;
        awsCase.date = activeCase.date;
        awsCase.location = activeCase.location;
        awsCase.locationNotes = activeCase.locationNotes;
        awsCase.photoTaken = activeCase.photoTaken;
        awsCase.photoNotes = activeCase.photoNotes;

        string filePath = Application.persistentDataPath + "/case#" + awsCase.caseID + ".dat";
        BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(filePath);

        using (var file = File.Open(filePath, FileMode.OpenOrCreate))
        {
            bf.Serialize(file, awsCase);
            Debug.Log("Application Data Path : " + filePath);
        } // file is automatically closed after reaching the end of the using block


        //Send to AWS
        AWSManager.Instance.UploadToS3(filePath, awsCase.caseID);
    }
}
