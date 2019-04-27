using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePhotoPanel : MonoBehaviour, IPanel
{
    public RawImage photoTaken;
    public InputField photoNotes;
    public GameObject overviewContainer;
    public Text caseNumberTitle;

    private string imgPath;

    void OnEnable()
    {
        caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
    }

    public void TakePictureButton()
    {
        TakePicture(512);
    }

    public void ProcessInfo()
    {
        //Create a 2D Texture
        //Apply the Texture from image path
        //encode to PNG
        //store bytes to PhotoTaken

        byte[] imgData = null;

        if (!string.IsNullOrEmpty(imgPath))
        {
            Texture2D img = NativeCamera.LoadImageAtPath(imgPath, 512, false);

            imgData = img.EncodeToPNG();
        }

        UIManager.Instance.activeCase.photoNotes = photoNotes.text;
        UIManager.Instance.activeCase.photoTaken = imgData;

        overviewContainer.SetActive(true);
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    return;
                }

                photoTaken.texture = texture;
                photoTaken.gameObject.SetActive(true);
                imgPath = path;
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }
}
