using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public World World;

    public GameObject MainMenu;

    public GameObject Player;

    public static GameController Instance;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {

    }

    public void StartGame()
    {
        World.enabled = true;
        World.chunksPerFrame = 8;
    }

    public void CloseMainMenu()
    {
        World.chunksPerFrame = 1;
        World.LoadingText.text = "";
        MainMenu.SetActive(false);
        Player.SetActive(true);
    }

}