using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWorldPanel : MonoBehaviour
{

    public WorldButton WorldB;

    public GameObject PanelRoot;

    public static CreateWorldPanel Instance;

    private void Start()
    {
        Instance = this;
    }

    public void CreateWorld(int worldtype)
    {
        WorldB.worldType = (WorldType)worldtype;
        PanelRoot.SetActive(false);
        Debug.Log("Creating world, type " + WorldB.worldType.ToString());
        World.Instance.Type = WorldB.worldType;
        World.Instance.Name = WorldB.WorldName.text;
        GameController.Instance.StartGame();
    }

}