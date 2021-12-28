using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System;
using System.IO.Compression;

/// <summary>
/// World list UI class
/// </summary>
public class WorldManager : MonoBehaviour // this is really old class, but i'm lazy to rewrite code :p
{
    public GameObject World_prefab;

    public RectTransform WorldsRoot;

    public int selectedWorld;

    public int CountWorlds;

    public Button NextWorldB;

    public Button LastWorldB;

    public List<WorldButton> Worlds;

    //Menu panels
    public GameObject[] LocalWorldsPanel;

    public GameObject[] OnlineWorldsPanel;

    public GameObject[] SettingsPanel;

    int SearchType = 1;

    //Toggle panels
    int typeMenu;

    //Preview of online world
    public GameObject PreviewObject;
    public Text NameWorldPreview;
    public string CurrentOnlineWorld;
    public Text SizeOfWorld;

    void Start()
    {
        //Session.isMultiplayer = false;
        InitWorlds();
        //InitNewWorlds();
        NextPosOnlineWorlds = OnlineWorldsRoot.anchoredPosition;
    }

    public void OpenPreviewWorld(Texture previewImage)
    {
        ToggleMenu(3);
        PreviewObject.SetActive(true);

        PreviewObject.GetComponent<RawImage>().texture = previewImage;
        OnlineWorldsPanel[2].SetActive(true);
        OnlineWorldsPanel[3].SetActive(true);
        for (int g = 0; g < SettingsPanel.Length; g++)
        {
            SettingsPanel[g].SetActive(false);
        }
    }
    public void BackPreviewWorld()
    {
        ToggleMenu(1);
        PreviewObject.SetActive(false);

    }

    public void DownloadOnlineWorld()
    {
        StartCoroutine(DownloadWorld(CurrentOnlineWorld));
    }


    public void ToggleMenu(int type)
    {
        typeMenu = type;
        for (int i = 0; i < LocalWorldsPanel.Length; i++)
        {
            if (type == 0)
            {
                ReloadWorlds();
                LocalWorldsPanel[i].SetActive(true);
                PreviewObject.SetActive(false);
                NextPosOnlineWorlds.y = 328; // hardcoded number lol, later fix it
            }
            else
            {
                LocalWorldsPanel[i].SetActive(false);
            }
        }

        for (int j = 0; j < OnlineWorldsPanel.Length; j++)
        {
            if (type == 1)
            {
                OnlineWorldsPanel[j].SetActive(true);
                PreviewObject.SetActive(false);
            }
            else
            {
                OnlineWorldsPanel[j].SetActive(false);
            }
        }
        for (int g = 0; g < SettingsPanel.Length; g++)
        {
            if (type == 2)
            {
                SettingsPanel[g].SetActive(true);
                PreviewObject.SetActive(false);
            }
            else
            {
                SettingsPanel[g].SetActive(false);
            }
        }
    }
    public Text SearchTypeText;
    public GameObject InputFieldSearch;

    public void NextTypeSearch()
    {
        for (int i = 0; i < ButtonsOnlineWorlds.Count; i++)
        {
            Destroy(ButtonsOnlineWorlds[i]);
        }
        ButtonsOnlineWorlds.Clear();
        SearchType++;
        if (SearchType >= 1)
        {
            SearchType = 0;
        }
        if (SearchType == 1)
        {
            SearchPopularWorlds();
        }

    }
    public void LastTypeSearch()
    {
        for (int i = 0; i < ButtonsOnlineWorlds.Count; i++)
        {
            Destroy(ButtonsOnlineWorlds[i]);
        }
        ButtonsOnlineWorlds.Clear();
        SearchType--;
        if (SearchType <= 0)
        {
            SearchType = 1;
        }
        if (SearchType == 1)
        {
            SearchPopularWorlds();
        }
    }
    public Button UpListWorldButton;
    public void UpListOnlineWorlds()
    {
        if (NextPosOnlineWorlds.y >= 328)
        {
            NextPosOnlineWorlds.y -= 188;
        }
    }
    public void DownListOnlineWorlds()
    {
        NextPosOnlineWorlds.y += 188;
    }

    //-----------------------------------------------------

    public void ReloadWorlds()
    {
        for (int i = 0; i < Worlds.Count; i++)
        {
            Destroy(Worlds[i].gameObject);
        }
        Worlds.Clear();
        CountWorlds = 0;
        InitWorlds();
       // InitNewWorlds();
    }

    public void CreateWorld()
    {
        int s = selectedWorld;
        var world = Instantiate(World_prefab, WorldsRoot);
        CountWorlds++;
        world.GetComponent<WorldButton>().WorldName.text = "World" + CountWorlds;
       // world.GetComponent<WorldButton>().isConvertedWorld = false;
        world.GetComponent<WorldButton>().isNew = true;
        Worlds.Add(world.GetComponent<WorldButton>());
        selectedWorld = CountWorlds;
        for (int i = 0; i < CountWorlds - s; i++)
        {
            NextPosWorlds.x -= 188;
        }
    }



    public void DeleteCurrentWorld()
    {
        if (CountWorlds > 0 && selectedWorld > 0)
        {
            File.Delete(Application.persistentDataPath + "/" + Worlds[selectedWorld - 1].NameFile + ".eden");
            Debug.Log("Deleting world... " + Application.persistentDataPath + "/" + Worlds[selectedWorld - 1].NameFile + ".eden");
            Destroy(Worlds[selectedWorld - 1].gameObject);
            Worlds.RemoveAt(selectedWorld - 1);
            CountWorlds--;
            selectedWorld--;
            if (selectedWorld <= 1)
            {
                
            }
            else
            {
                NextPosWorlds.x += 188;
            }
        }
    }

    public void InitWorlds()
    {
        //Loading old worlds
        //Debug.Log("Directory all old worlds: " + Application.persistentDataPath);
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.eden");
        foreach (FileInfo f in info)
        {
            var world = Instantiate(World_prefab, WorldsRoot);
            CountWorlds++;
            Worlds.Add(world.GetComponent<WorldButton>());
            byte[] bytes;
            using (FileStream stream = new FileStream(f.ToString(), FileMode.Open))
            {
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
            }

            byte[] nameArray = bytes.TakeWhile((b, i) => (i < 40 || b != 0)).ToArray();
            string worldName = Encoding.ASCII.GetString(nameArray, 40, nameArray.Length - 40);
            world.GetComponent<WorldButton>().WorldName.text = worldName;
            world.GetComponent<WorldButton>().NameFile = Path.GetFileNameWithoutExtension(f.ToString());
           // world.GetComponent<WorldButton>().NameFileEx = Path.GetFileName(f.ToString());
           // world.GetComponent<WorldButton>().isConvertedWorld = true;
        }

    }
    /*
    public void InitNewWorlds()
    {
       // Debug.Log("Directory all new worlds: " + Application.persistentDataPath);
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.vm");
        foreach (FileInfo f in info)
        {
            var world = Instantiate(World_prefab, WorldsRoot);
            CountWorlds++;
            Worlds.Add(world.GetComponent<WorldButton>());
            byte[] bytes;
            using (FileStream stream = new FileStream(f.ToString(), FileMode.Open))
            {
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
            }
            world.GetComponent<WorldButton>().WorldName.text = Path.GetFileNameWithoutExtension(f.ToString());
            world.GetComponent<WorldButton>().Namefile = Path.GetFileNameWithoutExtension(f.ToString());
            world.GetComponent<WorldButton>().NameFileEx = Path.GetFileName(f.ToString());
            world.GetComponent<WorldButton>().isConvertedWorld = false;
        }
    }
    */
    public void NextWorld()
    {
        if (selectedWorld < CountWorlds)
        {
            selectedWorld += 1;
            NextPosWorlds.x -= 188;
        }
    }
    public void LastWorld()
    {
        if (selectedWorld > 0)
        {
            selectedWorld -= 1;
            NextPosWorlds.x += 188;
        }

    }
    Vector2 NextPosWorlds;
    Vector2 NextPosOnlineWorlds;
    void Update()
    {
        //Moving lists
        WorldsRoot.anchoredPosition = Vector2.MoveTowards(WorldsRoot.anchoredPosition, NextPosWorlds, 16f);
        OnlineWorldsRoot.anchoredPosition = Vector2.MoveTowards(OnlineWorldsRoot.anchoredPosition, NextPosOnlineWorlds, 16f);

        if (Worlds.Count > 0)
        {
            for (int i = 0; i < Worlds.Count; i++)
            {
                Worlds[i].WorldName.enabled = false;
                Worlds[i].btn.interactable = false;
                Worlds[i].transform.localScale = new Vector3(0.77f, 0.77f, 0.77f);
            }
            Worlds[selectedWorld - 1].WorldName.enabled = true;
            Worlds[selectedWorld - 1].btn.interactable = true;
            Worlds[selectedWorld - 1].transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }

        if (NextPosOnlineWorlds.y > 328)
        {
            UpListWorldButton.interactable = true;
        }
        else
        {
            UpListWorldButton.interactable = false;
        }

        //Search types
        if (SearchType == 0)
        {
            SearchTypeText.text = "Search for name";
            InputFieldSearch.SetActive(true);
        }
        else if (SearchType == 1)
        {
            SearchTypeText.text = "Sorted by popular";
            InputFieldSearch.SetActive(false);
        }
        //----

        if (selectedWorld < CountWorlds)
        {
            NextWorldB.interactable = true;
        }
        else
        {
            NextWorldB.interactable = false;
        }

        if (selectedWorld > 1)
        {
            LastWorldB.interactable = true;
        }
        else
        {
            LastWorldB.interactable = false;
        }
    }
    public GameObject PrefabOnlineWorldButton;
    public RectTransform OnlineWorldsRoot;
    public void SearchWorlds(InputField field)
    {
        for (int i = 0; i < ButtonsOnlineWorlds.Count; i++)
        {
            Destroy(ButtonsOnlineWorlds[i]);
        }
        ButtonsOnlineWorlds.Clear();
        StartCoroutine(SearchWorld(field.text));
    }

    public void SearchPopularWorlds()
    {
        for (int i = 0; i < ButtonsOnlineWorlds.Count; i++)
        {
            Destroy(ButtonsOnlineWorlds[i]);
        }
        ButtonsOnlineWorlds.Clear();

        StartCoroutine(PopularWorlds());
    }

    List<GameObject> ButtonsOnlineWorlds = new List<GameObject>();
    IEnumerator SearchWorld(string search)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://app.edengame.net/list2.php?search=" + search);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string LoadedNames = www.downloadHandler.text;
            string[] worldsName = new string[0];
            worldsName = LoadedNames.Split('\n');
            string id = "";

            for (int i = 0; i < worldsName.Length; i++)
            {
                if (worldsName[i].Contains(".eden"))
                {
                    id = worldsName[i];
                }
                if (worldsName[i].Contains(".name"))
                {
                    string resultName = worldsName[i].Replace(".name", "");
                    var world = Instantiate(PrefabOnlineWorldButton, OnlineWorldsRoot);
                    ButtonsOnlineWorlds.Add(world);
                    world.GetComponent<OnlineWorldButton>().wm = this;
                    world.GetComponent<OnlineWorldButton>().WorldName = resultName;
                    world.GetComponent<OnlineWorldButton>().IDWorld = id;

                }
            }
        }
    }

    IEnumerator PopularWorlds()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://edengame.net/popularlist.txt");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string LoadedNames = www.downloadHandler.text;
            string[] worldsName = new string[0];
            worldsName = LoadedNames.Split('\n');
            string id = "";

            for (int i = 0; i < worldsName.Length; i++)
            {
                if (worldsName[i].Contains(".eden"))
                {
                    id = worldsName[i];
                }
                if (worldsName[i].Contains(".name"))
                {

                    string resultName = worldsName[i].Replace(".name", "");
                    var world = Instantiate(PrefabOnlineWorldButton, OnlineWorldsRoot);
                    ButtonsOnlineWorlds.Add(world);
                    world.GetComponent<OnlineWorldButton>().wm = this;
                    world.GetComponent<OnlineWorldButton>().WorldName = resultName;
                    world.GetComponent<OnlineWorldButton>().IDWorld = id;

                }
            }
        }
    }

    public IEnumerator DownloadWorld(string n)
    {
        var uwr = new UnityWebRequest("http://files.edengame.net/" + n, UnityWebRequest.kHttpVerbGET);
        string path = Application.persistentDataPath + "/" + n + "temp";

        uwr.downloadHandler = new DownloadHandlerFile(path);

        NameWorldPreview.text = "Downloading world... " + -uwr.downloadProgress * 100 + "%";
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            Debug.Log("World successfully downloaded and saved to " + path);
            Debug.Log("Downloaded bytes: " + uwr.downloadedBytes);
            DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] info = dir.GetFiles(n + "temp");
            foreach (FileInfo f in info)
            {
                DecompressGZip(f);
            }

            BackPreviewWorld();
        }

    }

    //Decompress worlds
    public void DecompressGZip(FileInfo fileToDecompress)
    {
        using (FileStream originalFileStream = fileToDecompress.OpenRead())
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
            newFileName += ".eden";

            using (FileStream decompressedFileStream = File.Create(newFileName))
            {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                }
            }
        }
        fileToDecompress.Delete();
    }


}