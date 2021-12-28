using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR // Simulating android touches for editor
using Input = InputWrapper.Input;
#endif

public class BuildController : MonoBehaviour
{

    public BlockType SelectedBlock;

    public Paintings SelectedColor;

    public int SelectedTool;

    public BlockPreview Preview;

    public GameObject TouchField;

    public HighlighterController Highlighter;

    public GameObject[] SelectedTexture;

    public bool isLocalPlayer = false;

    private World _world;

    private int _lastTool;

    private int _angle;

    public static BuildController Instance;

    public GameObject InventoryBlocks;

    public GameObject InventoryColors;

    public GameObject PaintingButton_Prefab;

    public SoundController PaintSounds;

    public ActionSoundController ActionSounds;

    public MusicController Music;

    public AudioClip MatchLightSound;

    void Start()
    {
        _world = World.Instance;
        Instance = this;
        SelectedTool = 4;
        InitPaintings();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == TouchField)
        {
            MultiTouchUpdate();
        }

        if (transform.position.y < 20.0f)
        {
            if (Music.AmbienceSource.clip != Music.Ambiences[1])
            {
                Music.AmbienceSource.enabled = false;
            }
            Music.AmbienceSource.clip = Music.Ambiences[1];
            Music.AmbienceSource.enabled = true;
        }
        else
        {
            if (Music.AmbienceSource.clip != Music.Ambiences[0])
            {
                Music.AmbienceSource.enabled = false;
            }
            Music.AmbienceSource.clip = Music.Ambiences[0];
            Music.AmbienceSource.enabled = true;
        }
    }

    public void MultiTouchUpdate()
    {
        float distanceRaycsat = 8.0f;
        Touch[] myTouches = Input.touches;
        if (myTouches.Length == 0)
        {
            if (Highlighter.Highlighters.Count > 0)
            {
                Highlighter.RemoveHighlights();
            }
        }

        for (int i = 0; i < myTouches.Length; i++)
        {
            Ray r = Camera.main.ScreenPointToRay(myTouches[i].position);
            RaycastHit hit;
            Physics.Raycast(r, out hit);
            if (SelectedTool == 2) // build
            {
                if (myTouches[i].phase == TouchPhase.Began)
                {
                    if (hit.collider != null && hit.distance < distanceRaycsat && hit.collider.CompareTag("Chunk"))
                    {
                        Vector3Int pos = Vector3Int.CeilToInt(hit.point + (hit.normal * 0.5f));
                        Highlighter.AddHightlight(pos - new Vector3(1f, 1f, 1f), false);
                    }
                }
                else if (myTouches[i].phase == TouchPhase.Ended)
                {
                    Vector3Int pos = Highlighter.GetHighlightPosition(myTouches[i]) + new Vector3Int(0, 1, 0);
                    _world.SetBlock(pos.x, pos.y, pos.z, SelectedBlock);
                    Chunk chunk = _world.FindChunk(pos.x, pos.y, pos.z);
                    chunk.RefreshAsync();
                    chunk.SetDirty();
                    Highlighter.RemoveHighlight(myTouches[i]);
                    PlayBlockSound(pos.x, pos.y, pos.z, Constants.Action.Build);
                }
            }
            else if (SelectedTool == 1) // remove
            {
                if (myTouches[i].phase == TouchPhase.Began)
                {
                    if (hit.collider != null && hit.distance < distanceRaycsat && hit.collider.CompareTag("Chunk"))
                    {
                        Vector3Int pos = Vector3Int.CeilToInt(hit.point - (hit.normal * 0.5f));
                        Highlighter.AddHightlight(pos - new Vector3(1f, 1f, 1f), true);
                    }
                }
                else if (myTouches[i].phase == TouchPhase.Ended)
                {
                    Vector3Int pos = Highlighter.GetHighlightPosition(myTouches[i]) + new Vector3Int(0, 1, 0);
                    if (_world.GetBlock(pos.x, pos.y, pos.z).BlockType != BlockType.Bedrock && _world.GetBlock(pos.x, pos.y, pos.z).BlockType != BlockType.Air)
                    {
                        BreakedBlock BBreakedBlock = Instantiate((GameObject)Resources.Load("BreakedBlock"), pos + new Vector3(0.5f, -0.5f, 0.5f), Quaternion.identity).GetComponent<BreakedBlock>();
                        BBreakedBlock.transform.eulerAngles = new Vector3(-90, 0, 0);
                        BBreakedBlock.Block = _world.GetBlock(pos.x, pos.y, pos.z);
                        PlayBlockSound(pos.x, pos.y, pos.z, Constants.Action.Break);
                        _world.SetBlock(pos.x, pos.y, pos.z, BlockType.Air);
                        Chunk chunk = _world.FindChunk(pos.x, pos.y, pos.z);
                        chunk.RefreshAsync();
                        chunk.SetDirty();
                        _world.RefreshNearChunks(chunk, pos.x, pos.y, pos.z);
                    }
                    Highlighter.RemoveHighlight(myTouches[i]);
                }
            }
            else if (SelectedTool == 0) // burn
            {
                if (myTouches[i].phase == TouchPhase.Began)
                {
                    if (hit.collider != null && hit.distance < distanceRaycsat && hit.collider.CompareTag("Chunk"))
                    {
                        Vector3Int pos = Vector3Int.CeilToInt(hit.point - (hit.normal * 0.5f));
                        Highlighter.AddHightlight(pos - new Vector3(1f, 1f, 1f), true);
                    }
                }
                else if (myTouches[i].phase == TouchPhase.Ended)
                {
                    Vector3Int pos = Highlighter.GetHighlightPosition(myTouches[i]) + new Vector3Int(0, 1, 0);
                    Block block = _world.GetBlock(pos);
                    BlockSet.BlockSettings b;
                    BlockSet.Blocks.TryGetValue(block.BlockType, out b);
                    if (b.isFlamming && block.isBurns == false)
                    {
                        hit.collider.gameObject.GetComponent<BurnChunkManager>().SetBurnV(pos.x, pos.y, pos.z);
                    }

                    Highlighter.RemoveHighlight(myTouches[i]);
                    ActionSounds.GetComponent<AudioSource>().PlayOneShot(MatchLightSound);
                }
            }
            else if (SelectedTool == 3) // paint
            {
                if (myTouches[i].phase == TouchPhase.Began)
                {
                    if (hit.collider != null && hit.distance < distanceRaycsat && hit.collider.CompareTag("Chunk"))
                    {
                        Vector3Int pos = Vector3Int.CeilToInt(hit.point - (hit.normal * 0.5f));
                        Highlighter.AddHightlight(pos - new Vector3(1f, 1f, 1f), true);
                    }
                }
                else if (myTouches[i].phase == TouchPhase.Ended)
                {
                    Vector3Int pos = Highlighter.GetHighlightPosition(myTouches[i]) + new Vector3Int(0, 1, 0);
                    _world.SetColor(pos.x, pos.y, pos.z, SelectedColor);
                    Chunk chunk = _world.FindChunk(pos.x, pos.y, pos.z);
                    chunk.RefreshAsync();
                    chunk.SetDirty();
                    Highlighter.RemoveHighlight(myTouches[i]);
                    PaintSounds.PlayRandomSound();
                    if (hit.collider.gameObject.name == "SkyTrigger")
                    {
                        SkyManager.Instance.Set(SelectedColor);
                    }
                }
            }
        }
    }

    public void InitPaintings()
    {
        int count = PaintController.PaintingDictonary.Count;
        for (int i = 1; i < count; i++)
        {
            Instantiate(PaintingButton_Prefab, InventoryColors.transform).GetComponent<SelectPainting>().Init((Paintings)i);
        }
    }

    public void PlayBlockSound(int x, int y, int z, Constants.Action action)
    {
        if (_world.GetBlock(x, y, z) != null)
        {
            BlockSet.BlockSettings settings;
            BlockSet.Blocks.TryGetValue(_world.GetBlock(x, y, z).BlockType, out settings);
            if (settings != null)
            {
                ActionSounds.PlayRandomSound(settings.BlockSound, action);
            }
            else
            {
                ActionSounds.PlayRandomSound(Constants.Blocks.Generic, action);
            }
        }
    }

    public void UpdateBlockPreview()
    {
        BlockSet.BlockSettings blocksettings;

        BlockSet.Blocks.TryGetValue(SelectedBlock, out blocksettings);
        Preview.UpdateIcon(blocksettings);
    }

    public void SelectTool(int tool)
    {
        _lastTool = SelectedTool;

        SelectedTool = tool;

        if (_lastTool == SelectedTool && SelectedTool != 2 && SelectedTool != 3)
        {
            SelectedTool = 4;
        }
        for (int i = 0; i < SelectedTexture.Length; i++)
        {
            SelectedTexture[i].SetActive(false);
        }
        if (SelectedTool < 4)
        {
            SelectedTexture[tool].SetActive(true);
        }
    }
}
