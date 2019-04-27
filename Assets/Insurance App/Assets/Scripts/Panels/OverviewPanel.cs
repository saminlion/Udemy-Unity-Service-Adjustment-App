using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class OverviewPanel : MonoBehaviour,IPanel
{
    public Text caseNumberTitle;
    public Text nameTitle;
    public Text dateTitle;
    public RawImage locationImage;
    public Text locationNotes;
    public RawImage photoTaken;
    public Text photoNotes;

    public void OnEnable()
    {
        caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
        nameTitle.text = UIManager.Instance.activeCase.name;
        dateTitle.text = DateTime.Today.ToString();
        locationNotes.text = "LOCATION NOTES:\n" + UIManager.Instance.activeCase.locationNotes;
        photoNotes.text = "PHOTO NOTES:\n" + UIManager.Instance.activeCase.photoNotes;

        //Reconstruct byte
        //Convert bytes to png
        //Convert Texture2D to texture

        Texture2D reconstructedPhotoImage = new Texture2D(1, 1);
        reconstructedPhotoImage.LoadImage(UIManager.Instance.activeCase.photoTaken);

        Texture2D reconstructedLocationImage = new Texture2D(1, 1);
        reconstructedLocationImage.LoadImage(UIManager.Instance.activeCase.location);

        locationImage.texture = reconstructedLocationImage as Texture;
        photoTaken.texture = reconstructedPhotoImage as Texture;
    }
    public void ProcessInfo()
    {

    }
}
