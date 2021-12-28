using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollTexture : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    Renderer img;
    public bool isRotate;
    void Start()
    {
        img = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        if (!isRotate)
        {
            Vector2 textureOffset = new Vector2(Time.time * scrollSpeed, 0);
            img.material.mainTextureOffset = textureOffset;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, -Time.time * scrollSpeed);
        }
    }
}
