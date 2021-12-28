using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{

    float r;

    void Start()
    {
        
    }

    void Update()
    {
        r = Mathf.Sin(Time.time * 0.3f);
        transform.Translate(Vector3.forward * 16f * Time.deltaTime);
        transform.eulerAngles = new Vector3(0,r*60,0);
    }
}
