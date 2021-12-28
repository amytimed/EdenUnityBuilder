using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
/// <summary>
/// Download world from webserver
/// </summary>
public class OnlineWorldButton : MonoBehaviour
{
    public string WorldName;

    public string IDWorld;

    public Text nameText;

    public WorldManager wm;

    void Start()
    {
        nameText.text = WorldName;
    }

    public void DownloadWorld()
    {
        wm.StartCoroutine("DownloadWorld", IDWorld);
    }

    public void OpenPreview()
    {
        wm.CurrentOnlineWorld = IDWorld;
        StartCoroutine(GetIcon(IDWorld));
        //StartCoroutine(GetFileSize(IDWorld)); 
    }

    IEnumerator GetIcon(string n)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://files.edengame.net/" + n + ".png");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            wm.NameWorldPreview.text = nameText.text;
            wm.OpenPreviewWorld(myTexture);
        }
    }

    IEnumerator GetFileSize(string n)
    {
        UnityWebRequest webRequest = UnityWebRequest.Head("http://files.edengame.net/" + n);
        webRequest.Send();
        while (!webRequest.isDone)
        {
            yield return null;
        }
        wm.SizeOfWorld.text = (int.Parse(webRequest.GetResponseHeader("Content-Length")) / 1000).ToString() + " KB";
    }
}