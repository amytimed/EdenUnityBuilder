using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintController : MonoBehaviour
{

    public static Dictionary<Paintings, Color> PaintingDictonary;

    void Awake()
    {
        PaintingDictonary = new Dictionary<Paintings, Color>();
        InitPaintings();
    }

    public static Color GetColor(Paintings color)
    {
        Color c = new Color(0,0,0,0);
        bool i = PaintingDictonary.TryGetValue(color, out c);
        if (i)
        {
            return c;
        }
        else
        {
            return Color.white;
        }
    }

    private void InitPaintings() // hardcoded colors lol
    {
        PaintingDictonary.Add(Paintings.Unpainted, new Color(1f, 1f, 1f, 1f));
        PaintingDictonary.Add(Paintings.LightRed, new Color(1f, 0.6666667f, 0.6666667f, 0f));
        PaintingDictonary.Add(Paintings.LightOrange, new Color(1f, 0.9176471f, 0.6666667f, 0f));
        PaintingDictonary.Add(Paintings.LightYellow, new Color(0.9843138f, 1f, 0.6666667f, 0f));
        PaintingDictonary.Add(Paintings.LightGreen, new Color(0.6666667f, 1f, 0.7490196f, 0f));
        PaintingDictonary.Add(Paintings.LightCyan, new Color(0.6666667f, 1f, 1f, 0f));
        PaintingDictonary.Add(Paintings.LightBlue, new Color(0.6666667f, 0.7490196f, 1f, 0f));
        PaintingDictonary.Add(Paintings.LightPurple, new Color(0.8313726f, 0.6666667f, 1f, 0f));
        PaintingDictonary.Add(Paintings.LightPink, new Color(1f, 0.6666667f, 0.9176471f, 0f));
        PaintingDictonary.Add(Paintings.LightGray_White, new Color(1f, 1f, 1f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightRed, new Color(1f, 0.3333333f, 0.3333333f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightOrange, new Color(1f, 0.8313726f, 0.3333333f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightYellow, new Color(0.9686275f, 1f, 0.3333333f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightGreen, new Color(0.3333333f, 1f, 0.5019608f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightCyan, new Color(0.3333333f, 1f, 1f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightBlue, new Color(0.3333333f, 0.5019608f, 1f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightPurple, new Color(0.6666667f, 0.3333333f, 1f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightPink, new Color(1f, 0.3333333f, 0.8313726f, 0f));
        PaintingDictonary.Add(Paintings.MediumLightGray, new Color(0.8000001f, 0.8000001f, 0.8000001f, 0f));
        PaintingDictonary.Add(Paintings.Red, new Color(1f, 0f, 0f, 0f));
        PaintingDictonary.Add(Paintings.Orange, new Color(1f, 0.7490196f, 0f, 0f));
        PaintingDictonary.Add(Paintings.Yellow, new Color(0.9490197f, 1f, 0f, 0f));
        PaintingDictonary.Add(Paintings.Green, new Color(0f, 1f, 0.2509804f, 0f));
        PaintingDictonary.Add(Paintings.Cyan, new Color(0f, 1f, 1f, 0f));
        PaintingDictonary.Add(Paintings.Blue, new Color(0f, 0.2509804f, 1f, 0f));
        PaintingDictonary.Add(Paintings.Purple, new Color(0.5019608f, 0f, 1f, 0f));
        PaintingDictonary.Add(Paintings.Pink, new Color(1f, 0f, 0.7490196f, 0f));
        PaintingDictonary.Add(Paintings.Gray, new Color(0.6f, 0.6f, 0.6f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkRed, new Color(0.7490196f, 0f, 0f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkOrange, new Color(0.7490196f, 0.5607843f, 0f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkYellow, new Color(0.7137255f, 0.7490196f, 0f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkGreen, new Color(0f, 0.7490196f, 0.1882353f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkCyan, new Color(0f, 0.7490196f, 0.7490196f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkBlue, new Color(0f, 0.1882353f, 0.7490196f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkPurple, new Color(0.3764706f, 0f, 0.7490196f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkPink, new Color(0.7490196f, 0f, 0.5607843f, 0f));
        PaintingDictonary.Add(Paintings.MediumDarkGray, new Color(0.4f, 0.4f, 0.4f, 0f));
        PaintingDictonary.Add(Paintings.DarkRed, new Color(0.5019608f, 0f, 0f, 0f));
        PaintingDictonary.Add(Paintings.DarkOrange, new Color(0.5019608f, 0.3764706f, 0f, 0f));
        PaintingDictonary.Add(Paintings.DarkYellow, new Color(0.4745098f, 0.5019608f, 0f, 0f));
        PaintingDictonary.Add(Paintings.DarkGreen, new Color(0f, 0.5019608f, 0.1254902f, 0f));
        PaintingDictonary.Add(Paintings.DarkCyan, new Color(0f, 0.5019608f, 0.5019608f, 0f));
        PaintingDictonary.Add(Paintings.DarkBlue, new Color(0f, 0.1254902f, 0.5019608f, 0f));
        PaintingDictonary.Add(Paintings.DarkPurple, new Color(0.2509804f, 0f, 0.5019608f, 0f));
        PaintingDictonary.Add(Paintings.DarkPink, new Color(0.5019608f, 0f, 0.3764706f, 0f));
        PaintingDictonary.Add(Paintings.DarkGray, new Color(0.2f, 0.2f, 0.2f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkRed, new Color(0.2509804f, 0f, 0f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkOrange, new Color(0.2509804f, 0.1882353f, 0f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkYellow, new Color(0.2392157f, 0.2509804f, 0f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkGreen, new Color(0f, 0.2509804f, 0.0627451f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkCyan, new Color(0f, 0.2509804f, 0.2509804f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkBlue, new Color(0f, 0.0627451f, 0.2509804f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkPurple, new Color(0.1254902f, 0f, 0.2509804f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkPink, new Color(0.2509804f, 0f, 0.1882353f, 0f));
        PaintingDictonary.Add(Paintings.VeryDarkGray_Black, new Color(0f, 0f, 0f, 0f));
    }
}
