using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Loading/Converting and Saving .eden files (File Format for Eden World Builder)
/// </summary>
public class EdenWorldDecoder : MonoBehaviour // Based on https://mrob.com/pub/vidgames/eden-file-format.html
{

    public string Name;
    public Dictionary<int, Vector2Int> Chunks;
    public static int skyColor;
    public static string worldName;

    [HideInInspector]
    public byte[] Bytes;

    private World world;

    public static EdenWorldDecoder Instance;

    string CurrentPathWorld;

    void Start()
    {
        Instance = this;
        world = World.Instance;
    }

    public void LoadWorld(string path)
    {
        CurrentPathWorld = path;
        List<int> skyColors = new List<int>();
        byte[] bytes;
        world.Name = Path.GetFileName(CurrentPathWorld);
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
        }

        // Get Sky Color
        for (int i = 132; i <= 148; i++)
        {
            if (bytes[i] != 14) skyColors.Add(bytes[i]);
        }

        if (skyColors.Count == 0) skyColor = 14;
        skyColor = skyColors.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;

        SkyManager.Instance.Set((Paintings)skyColor);
        SkyManager.Instance.FastUpdateSky();

        int chunkPointerStartIndex = bytes[35] * 256 * 256 * 256 + bytes[34] * 256 * 256 + bytes[33] * 256 + bytes[32];

        byte[] nameArray = bytes.TakeWhile((b, i) => ((i < 40 || b != 0) && i <= 75)).ToArray();
        worldName = Encoding.ASCII.GetString(nameArray, 40, nameArray.Length - 40);
        Vector4 worldArea = new Vector4(0, 0, 0, 0);
        Dictionary<int, Vector2Int> chunks = new Dictionary<int, Vector2Int>();
        Debug.Log("Loading world... " + worldName);
        // Create array of chunk points and addresses
        int currentChunkPointerIndex = chunkPointerStartIndex;
        do
        {
            chunks.Add(
                bytes[currentChunkPointerIndex + 11] * 256 * 256 * 256 + bytes[currentChunkPointerIndex + 10] * 256 * 256 + bytes[currentChunkPointerIndex + 9] * 256 + bytes[currentChunkPointerIndex + 8],//address
                new Vector2Int(bytes[currentChunkPointerIndex + 1] * 256 + bytes[currentChunkPointerIndex], bytes[currentChunkPointerIndex + 5] * 256 + bytes[currentChunkPointerIndex + 4])); //Position
        } while ((currentChunkPointerIndex += 16) < bytes.Length);

        //Get max dimensions of world
        worldArea.x = chunks.Values.Min(p => p.x);
        worldArea.y = chunks.Values.Min(p => p.y);
        worldArea.z = chunks.Values.Max(p => p.x) - worldArea.x + 1;
        worldArea.w = chunks.Values.Max(p => p.y) - worldArea.y + 1;
        Bytes = bytes;
        Chunks = chunks;
        WorldArea = worldArea;
    }
    public Vector4 WorldArea;


    public bool HasChunk(Vector2Int Pos)
    {
        // Vector2Int posConverted = new Vector2Int((Pos.x - (int)WorldArea.x) * 16, (Pos.y - (int)WorldArea.y) * 16);
        // Debug.Log(posConverted);
        Vector2Int ConvertedPosNew = new Vector2Int((Pos.y / 16) + (int)WorldArea.x, (Pos.x / 16) + (int)WorldArea.y);
        if (Chunks.ContainsValue(ConvertedPosNew))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector2Int ConvertToOldPosition(Vector2Int Pos)
    {
        return new Vector2Int((Pos.y / 16) + (int)WorldArea.x, (Pos.x / 16) + (int)WorldArea.y);
    }

    public Vector2Int ConvertToNewPosition(Vector2Int Pos)
    {
        return new Vector2Int((Pos.y * 16) - (int)WorldArea.y * 16, (Pos.x * 16) - (int)WorldArea.x * 16);
    }

    public Vector2 ConvertToNewPositionFloat(Vector2Int Pos)
    {
        return new Vector2((Pos.y * 16) - (int)WorldArea.y * 16, (Pos.x * 16) - (int)WorldArea.x * 16);
    }

    public Vector3 ConvertToNewPosition3(Vector2Int Pos)
    {
        return new Vector3((Pos.y * 16) - (int)WorldArea.y * 16, 0, (Pos.x * 16) - (int)WorldArea.x * 16);
    }

    public void LoadChunk(Vector3 Pos)
    {
        //  foreach (int address in Chunks.Keys)
        if (HasChunk(new Vector2Int((int)Pos.x, (int)Pos.z)))
        {
            Vector2Int chunk = ConvertToOldPosition(new Vector2Int((int)Pos.x, (int)Pos.z));
            int address = Chunks.Single(s => s.Value == chunk).Key;

            int globalChunkPosX = chunk.x;

            int globalChunkPosY = chunk.y;

            var realChunkPosX = (globalChunkPosX * 16) * 100;

            var realChunkPosY = (globalChunkPosY * 16) * 100;

            var baseX = (chunk.x - WorldArea.x) * 16;

            var baseY = (chunk.y - WorldArea.y) * 16;

            for (int BaseHeight = 0; BaseHeight < 4; BaseHeight++)
            {

                if (world.ChunkExists(new Vector3((int)baseX, 16 * BaseHeight, (int)baseY)) && world.FindChunk(new Vector3((int)baseX, 16 * BaseHeight, (int)baseY)).isConverted == false)
                {
                    Chunk c = world.CreateChunk(new Vector3((int)baseX, 16 * BaseHeight, (int)baseY));
                    c.InitData();
                    for (int x = 0; x < 16; x++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            for (int y = 0; y < 16; y++)
                            {
                                var id = Bytes[address + BaseHeight * 8192 + x * 256 + y * 16 + z];
                                var color = Bytes[address + BaseHeight * 8192 + x * 256 + y * 16 + z + 4096];
                                var RealX = (x + (globalChunkPosX * 16));
                                var RealY = (y + (globalChunkPosY * 16));
                                var RealZ = (z + (16 * BaseHeight));

                                var Position = new Vector3Int(RealY - (int)WorldArea.y * 16, RealZ, RealX - (int)WorldArea.x * 16);

                                //Block spawn
                                c.SetBlock(x, z, y, (BlockType)id);
                                c.SetColor(x, z, y, (Paintings)color);
                            }
                        }
                    }

                    if (c != null && c.isConverted == false)
                    {
                        c.isConverted = true;
                        c.isDirty = true;
                        c.RefreshAsync();
                    }
                }
            }
        }
    }

    // This does not work as it requires chunk pointers which are not used in my version. I'm not sure how to fix this yet.
    // I think it's worth looking into this https://mrob.com/pub/vidgames/eden-file-format.html
    public void SaveWorld(string path)
    {
        CurrentPathWorld = path;
        if (!path.Contains(".eden")) // protection to prevent accidental overwriting of other files
        {
            Debug.Log("Wrong path for save: " + path);
            return;
        }

        foreach (int address in Chunks.Keys)
        {
            Vector2Int chunk = Chunks[address];
            var baseX = (chunk.x - WorldArea.x) * 16;

            var baseY = (chunk.y - WorldArea.y) * 16;
            //Vector2Int pos;
            //  Chunks.TryGetValue(address,out )
            for (int BaseHeight = 0; BaseHeight < 4; BaseHeight++)
            {
                Vector3 posChunk = new Vector3((int)ConvertToNewPositionFloat(chunk).y, (int)(BaseHeight * 16), (int)ConvertToNewPositionFloat(chunk).x);
                Chunk chunkForSave = world.FindChunk(posChunk);

                if (chunkForSave != null)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            for (int y = 0; y < 16; y++)
                            {
                                Bytes[address + BaseHeight * 8192 + x * 256 + y * 16 + z] = (byte)chunkForSave.GetBlock(x, z, y).BlockType;
                                Bytes[address + BaseHeight * 8192 + x * 256 + y * 16 + z + 4096] = (byte)chunkForSave.GetBlock(x, z, y).Painting;
                            }
                        }
                    }
                }
            }
        }

        if (File.Exists(path))
        {
            File.Delete(path);
        }


        using (FileStream stream = new FileStream(path, FileMode.CreateNew))
        {
            //Save File with changed world name or original world name
            byte[] name = Encoding.ASCII.GetBytes(worldName);
            for (int i = 0; i < Bytes.Length; i++)
            {
                if (i >= 40 && i <= (75))
                {
                    if (i - 40 < name.Length)
                    {
                        stream.WriteByte(name[i - 40]);
                    }
                    else
                    {
                        stream.WriteByte(0);
                    }

                }
                else
                {
                    stream.WriteByte(Bytes[i]);
                }

            }
        }
    }

}
