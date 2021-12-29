using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static bool Music;

    public static bool Sounds;

    public static bool Autojump;

    public static int Graphics;

    public static bool Creatures;
    public static float Sensitivity;
    public static float FOV;

    public SettingButton[] btns;
    public Slider[] sliders;

    public AudioSource MenuMusic;
    

    public void Save()
    {
        Music = btns[0].State;
        Sounds = btns[1].State;
        Autojump = btns[2].State;
        Graphics = (btns[3].State ? 1 : 0);
        Creatures = btns[4].State;
        Sensitivity = sliders[0].value;
        FOV = sliders[1].value;

        SetBool("Music", Music);
        SetBool("Sounds", Sounds);
        SetBool("Autojump", Autojump);
        PlayerPrefs.SetInt("Graphics", Graphics);
        SetBool("Creatures", Creatures);
        PlayerPrefs.SetFloat("Sensitivity", Sensitivity);
        PlayerPrefs.SetFloat("FOV", FOV);
        MenuMusic.enabled = Music;
    }

    public void Load()
    {
        Music = GetBool("Music");
        Sounds = GetBool("Sounds");
        Autojump = GetBool("Autojump");
        Graphics = PlayerPrefs.GetInt("Graphics");
        Creatures = GetBool("Creatures");
        Sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        FOV = PlayerPrefs.GetFloat("FOV");

        // nice, old code
        btns[0].State = Music;
        btns[1].State = Sounds;
        btns[2].State = Autojump;
        btns[3].State = IntToBool(Graphics);
        btns[4].State = Creatures;
        sliders[0].value = Sensitivity;
        sliders[1].value = FOV;
        for (int x = 0; x < btns.Length; x++)
        {
            btns[x].UpdateButtons();
        }
        MenuMusic.enabled = Music;
    }

    private void FixedUpdate()
    {
        if (Graphics == 0)
        {
            // Player.RenderDistance = 64;
        }
        else if (Graphics == 1)
        {
            // Player.RenderDistance = 128;
        }

    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            Load();
        }
        else
        {
            btns[0].State = true;
            btns[1].State = true;
            btns[2].State = true;
            btns[3].State = true;
            btns[4].State = true;
            sliders[0].value = 1f;
            sliders[1].value = 50f;
            Music = true;
            Sounds = true;
            Autojump = true;
            Graphics = 1;
            Creatures = true;
            Sensitivity = 1f;
            FOV = 50f;
        }
        for (int x = 0; x < btns.Length; x++)
        {
            btns[x].UpdateButtons();
        }
    }

    bool IntToBool(int value)
    {
        return value == 1 ? true : false;
    }

    void SetBool(string name, bool booleanValue)
    {
        PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
    }

    bool GetBool(string name)
    {
        return PlayerPrefs.GetInt(name) == 1 ? true : false;
    }

}