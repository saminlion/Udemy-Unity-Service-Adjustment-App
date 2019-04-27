using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class AWSManager : MonoBehaviour
{
    private static AWSManager _instance;
    public static AWSManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("AWS Manager is NULL");
            }
            return _instance;
        }
    }

    public string S3Region = RegionEndpoint.APNortheast2.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }

    private AmazonS3Client _s3Client;

    public AmazonS3Client S3Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(new CognitoAWSCredentials(
                "ap-northeast-2:31a5247c-005c-465d-89cb-259ec20c9eb2", // Identity Pool ID
                RegionEndpoint.APNortheast2 // Region
                ), _S3Region);
            }

            return _s3Client;
        }
    }

    private void Awake()
    {
        _instance = this;

        UnityInitializer.AttachToGameObject(this.gameObject);

        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        S3Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                responseObject.Response.Buckets.ForEach((s3b) =>
                {
                    Debug.Log("Bucket Name : " + s3b.BucketName);
                });
            }
            else
            {
                Debug.Log("AWS Error" + responseObject.Exception);
            }
        });
    }

    public void UploadToS3(string path, string caseID)
    {
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        PostObjectRequest request = new PostObjectRequest()
        {
            Bucket = "serviceappcasefileshong",
            Key = "case#"+caseID,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = _S3Region
        };

        S3Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log("Successfully posted to bucket");

                SceneManager.LoadScene(0);
            }

            else
            {
                Debug.Log("Exception Occured during Uploading : " + responseObj.Exception);
            }
        });
    }

    public void GetList(string caseNumber)
    {
        string target = "case#" + caseNumber;

        var request = new ListObjectsRequest()
        {
            BucketName = "serviceappcasefileshong"
        };

        S3Client.ListObjectsAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                bool caseFound = responseObj.Response.S3Objects.Any(obj => obj.Key == target);

                if (caseFound)
                {
                    Debug.Log("Case Found");

                    //download this object
                    //end this process

                    S3Client.GetObjectAsync("serviceappcasefileshong", target, (robj) =>
                    {
                        //read the data and apply it to a case (object) to be used
                        
                        //check if response stream is null
                        if (robj.Response.ResponseStream != null)
                        {
                            //byte arrary to store data from file
                            byte[] data = null;

                            //use streamreader to read response data
                            using (StreamReader reader = new StreamReader(robj.Response.ResponseStream))
                            {
                                //access a memory stream
                                using (MemoryStream memory = new MemoryStream())
                                {
                                    //populate data byte array with memstream data
                                    var buffer = new byte[512];

                                    var bytesRead = default(int);

                                    while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        memory.Write(buffer, 0, bytesRead);
                                    }

                                    data = memory.ToArray();
                                }
                            }

                            //convert those bytes to a Case (Object)

                            using (MemoryStream memory = new MemoryStream(data))
                            {
                                BinaryFormatter bf = new BinaryFormatter();

                                Case downloadedCase = bf.Deserialize(memory) as Case;

                                Debug.Log("Downloaded Case Name : " + downloadedCase.name);
                            }
                        }                                             
                    });
                }
                else
                {
                    Debug.Log("Case Not Found");
                }
            }

            else
            {
                Debug.Log("Error getting List of Items from S3 : " + responseObj.Exception);
            }
        });
    }
}
