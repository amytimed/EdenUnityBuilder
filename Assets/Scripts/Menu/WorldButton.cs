using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class WorldButton : MonoBehaviour
{

    public Text WorldName;

    public Button btn;

    public string NameFile;

    public WorldType worldType;

    public bool isNew;

    void Start()
    {

    }

    public void SelectNewWorld(int type)
    {
        worldType = (WorldType)type;
        World.Instance.Type = worldType;
        StartCoroutine(WorldLoad());
    }

    public void LoadWorld()
    {
        if (!isNew)
        {
            StartCoroutine(WorldLoad());
        }
        else
        {
            CreateWorldPanel.Instance.WorldB = this;
            CreateWorldPanel.Instance.PanelRoot.SetActive(true);
        }

    }

    IEnumerator WorldLoad()
    {
        EdenWorldDecoder.Instance.LoadWorld(Application.persistentDataPath + "/" + NameFile + ".eden");
        GameController.Instance.World.Type = WorldType.Flat;
        GameController.Instance.StartGame();
        yield return null;
    }
}