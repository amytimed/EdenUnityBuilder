using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingButton : MonoBehaviour
{
    public bool State;
    public Image On;
    public Image Off;

    private void Start()
    {
        On.gameObject.GetComponent<Button>().onClick.AddListener(delegate { Toggle(true); });
        Off.gameObject.GetComponent<Button>().onClick.AddListener(delegate { Toggle(false); });
    }
    public void Toggle(bool t)
    {
        State = t;
        UpdateButtons();
    }
    public void UpdateButtons()
    {
        if(State)
        {
            On.color = new Color32(0,255,0,255);
            Off.color = new Color32(180, 180, 180, 255);
        }
        else
        {
            On.color = new Color32(180, 180, 180, 255);
            Off.color = new Color32(255, 0, 0, 255);
        }
    }
}