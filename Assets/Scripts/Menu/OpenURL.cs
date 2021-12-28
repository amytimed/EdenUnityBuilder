using UnityEngine;
public class OpenURL : MonoBehaviour
{
    public void Open(string url)
    {
        Application.OpenURL(url);
    }
}