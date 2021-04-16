using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class WebAPI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetDate());
    }

    private IEnumerator GetDate()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("http://www.keiishkii.com/GetDate.php");

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("There was an error on the connection");
        }
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
        }
        
        yield return null;
    }
}
