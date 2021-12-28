using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public static int CurrentFPS = 0;

    void Update()
    {
        if ((Time.frameCount % 20) == 0)
        {
            CurrentFPS = (int)(1.0f / Time.smoothDeltaTime);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 400, 20),"FPS: " + CurrentFPS);
    }
}
