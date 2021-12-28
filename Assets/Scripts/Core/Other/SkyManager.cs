using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkyManager : MonoBehaviour // old class
{
    public Color Top;
    public Color Bottom;
    public MeshRenderer Renderer;
    public Light Sun;
    Color currentTop;
    Color currentBottom;
    bool isNight;

    public static SkyManager Instance;

     void SelectSkyColor(Color top, Color bottom)
    {
        Top = top;
        Bottom = bottom;
    }

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        currentTop = Renderer.material.GetColor("_TopColor");
        currentBottom = Renderer.material.GetColor("_BottomColor");
        Renderer.material.SetColor("_TopColor", Color.Lerp(currentTop, Top, 0.02f));
        Renderer.material.SetColor("_BottomColor", Color.Lerp(currentBottom, Bottom, 0.02f));
        RenderSettings.fogColor = currentBottom;
        isNight = Bottom.Equals(new Color(0, 0, 0, 1));
        if (isNight)
        {
            Sun.intensity = Mathf.Lerp(Sun.intensity, 0f, 0.02f);
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, new Color(0.2f, 0.2f, 0.2f, 1), 0.02f);
        }
        else
        {
            Sun.intensity = Mathf.Lerp(Sun.intensity, 0.5f, 0.02f);
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, new Color(0.5333334f, 0.5333334f, 0.5333334f, 1), 0.02f);
        }
    }

    public void Set(Paintings SelectedColor)
    {
        Color t = PaintController.GetColor(SelectedColor);
        Color skyTop = new Color(t.r / 5, t.g / 5, t.b / 5, 1);
        Color skyBottom = new Color(t.r, t.g, t.b, 1);
        if (SelectedColor != Paintings.MediumLightBlue)
        {
            SelectSkyColor(skyTop, skyBottom);
        }
        else
        {
            SelectSkyColor(new Color(0.2117647f, 0.4980392f, 0.8901961f, 1), new Color(0.7921569f, 0.8509804f, 0.9254902f, 1)); // default sky color
        }
    }

    public void FastUpdateSky()
    {
        currentTop = Renderer.material.GetColor("_TopColor");
        currentBottom = Renderer.material.GetColor("_BottomColor");
        Renderer.material.SetColor("_TopColor", Top);
        Renderer.material.SetColor("_BottomColor", Bottom);
        RenderSettings.fogColor = currentBottom;
        isNight = Bottom.Equals(new Color(0, 0, 0, 1));
        if (isNight)
        {
            Sun.intensity = 0;
            RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f, 1);
        }
        else
        {
            Sun.intensity = 0.4f;
            RenderSettings.ambientLight = new Color(0.5333334f, 0.5333334f, 0.5333334f, 1);
        }
    }
}