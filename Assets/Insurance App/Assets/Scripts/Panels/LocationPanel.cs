using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Networking;

public class LocationPanel : MonoBehaviour, IPanel
{
    public RawImage mapImage;
    public InputField mapNotes;
    public Text caseNumberTitle;

    public string apiKey;
    public float xCord, yCord;
    public int zoom;
    public int imgSize;
    public string url = "https://maps.googleapis.com/maps/api/staticmap?";

    void OnEnable()
    {
        caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
    }

    public IEnumerator Start()
    {
        //AndroidRuntimePermissions.Permission result = AndroidRuntimePermissions.RequestPermission("android.permission.ACCESS_FINE_LOCATION");
        //if (result == AndroidRuntimePermissions.Permission.Granted)
        //{
        //}
        //else
        //    Debug.Log("Permission state: " + result);


        if (GetPermission())
        {
            //Fetch Geo Data
            if (Input.location.isEnabledByUser)
            {
                //start service
                Input.location.Start();

                int maxWait = 20;

                while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
                {
                    yield return new WaitForSeconds(1.0f);
                    maxWait--;
                }

                if (maxWait < 1)
                {
                    Debug.Log("Timed Out");
                    yield break;
                }

                else if (Input.location.status == LocationServiceStatus.Failed)
                {
                    Debug.Log("Unable to determine device location...");
                }

                else
                {
                    xCord = Input.location.lastData.latitude;
                    yCord = Input.location.lastData.longitude;
                }

                Input.location.Stop();
            }
            else
            {
                Debug.Log("Location Services are not Enabled");
            }

            StartCoroutine(GetLocationRoutine());
        }
    }

    bool GetPermission()
    {
#if UNITY_ANDROID	
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
#endif
        return true;
    }

    IEnumerator GetLocationRoutine()
    {
        //Construct appropriate url
        url = url + "center=" + xCord + "," + yCord + "&zoom=" + zoom + "&size=" + imgSize + "x" + imgSize + "&key=" + apiKey;

        //Download static map
        //GoogleMap Download Url Example : https://maps.googleapis.com/maps/api/staticmap?center=40.714728,-73.998672&zoom=14&size=400x400&key=YOUR API KEY

        UnityWebRequest map = UnityWebRequestTexture.GetTexture(url);

        yield return map.SendWebRequest();

        if (map.isNetworkError || map.isHttpError)
        {
            Debug.LogError("Map Error : " + map.error);
        }
        else
        {
            //apply the map to the rawImage
            mapImage.texture = ((DownloadHandlerTexture)map.downloadHandler).texture;
            //Convert texture to Texture2D
            //Convert to bytes
            Texture2D convertedPhoto = mapImage.texture as Texture2D;
            byte[] imgData = convertedPhoto.EncodeToPNG();

            UIManager.Instance.activeCase.location = imgData;
        }
    }

    public void ProcessInfo()
    {
        if (!string.IsNullOrEmpty(mapNotes.text))
        {
            UIManager.Instance.activeCase.locationNotes = mapNotes.text;
        }


    }
}
