using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BurnChunkManager : MonoBehaviour
{

    public Chunk CurrentChunk;
    public bool isBurning;
    private int burnedVoxels;
    GameObject FireAudioSource;

    void Start()
    {
        CurrentChunk = GetComponent<Chunk>();
        StartCoroutine(CheckChunk());
    }

    IEnumerator CheckChunk()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            for (int x = 0; x < World.ChunkSize; x++)
            {
                for (int y = 0; y < World.ChunkSize; y++)
                {
                    for (int z = 0; z < World.ChunkSize; z++)
                    {
                        Block block = CurrentChunk.GetBlock(x, y, z);
                        BlockSet.BlockSettings b;
                        BlockSet.Blocks.TryGetValue(block.BlockType, out b);
                        if (b.isFlamming && block.isBurns == true)
                        {
                            CheckNeighborBlocks(x + CurrentChunk.Position.x, y + CurrentChunk.Position.y, z + CurrentChunk.Position.z);
                        }
                    }
                }

                if (isBurning)
                {
                    CurrentChunk.RefreshAsync();
                    isBurning = false;
                }

                if (burnedVoxels <= 0)
                {
                    burnedVoxels = 0;
                    isBurning = false;
                    Destroy(FireAudioSource);
                }
            }
        }
    }

    void CheckNeighborBlocks(int x, int y, int z)
    {
        if (World.Instance.GetBlock(x, y, z).isBurns == true)
        {
            if (World.Instance.GetBlock(x + 1, y, z).BlockType != BlockType.Air)
            {
                StartCoroutine(SetBurn(x + 1, y, z));
            }
            if (World.Instance.GetBlock(x - 1, y, z).BlockType != BlockType.Air)
            {
                StartCoroutine(SetBurn(x - 1, y, z));
            }
            if (World.Instance.GetBlock(x, y + 1, z).BlockType != BlockType.Air)
            {
                StartCoroutine(SetBurn(x, y + 1, z));
            }
            if (World.Instance.GetBlock(x, y - 1, z).BlockType != BlockType.Air)
            {
                StartCoroutine(SetBurn(x, y - 1, z));
            }
            if (World.Instance.GetBlock(x, y, z + 1).BlockType != BlockType.Air)
            {
                StartCoroutine(SetBurn(x, y, z + 1));
            }
            if (World.Instance.GetBlock(x, y, z - 1).BlockType != BlockType.Air)
            {
                StartCoroutine(SetBurn(x, y, z - 1));
            }
        }
    }

    public void SetBurnV(int x, int y, int z)
    {
        StartCoroutine(SetBurn(x, y, z));
    }

    public void CheckChunkV(int x, int y, int z)
    {
        GameObject Burn = Instantiate(Resources.Load("Burn"), new Vector3(x, y - 1, z), Quaternion.identity) as GameObject;
        if (FireAudioSource == null)
        {
            FireAudioSource = Instantiate(Resources.Load("FireAreaSound"), new Vector3(x, y, z), Quaternion.identity) as GameObject;
            Destroy(FireAudioSource, 2.2f);
        }
        Destroy(Burn, 2.2f);
        StartCoroutine(TimeDestroyBlock(x, y, z));
    }

    public void Explode(int x, int y, int z)
    {
        BlockSet.BlockSettings block;
        BlockSet.Blocks.TryGetValue(World.Instance.GetBlock(x, y - 1, z).BlockType, out block);
        ExplosionController.Instance.SpawnExplosionEffect(x, y, z, block.BlockSound);
        World.Instance.Sphere(new Vector3(x, y, z), 6, BlockType.Air, true);
        if (Vector3.Distance(PlayerMovement.Instance.transform.position, new Vector3(x, y, z)) < 10f)
        {
            Vector3 dir = PlayerMovement.Instance.transform.position + new Vector3(x, y, z);
            dir.y = 0;
            PlayerMovement.Instance.Explode(dir, 0.02f);
        }
        StartCoroutine(UpdateWait(x, y, z));
    }


    IEnumerator UpdateWait(int x, int y, int z)
    {
        Chunk[] chunks = World.Instance.GetNearChunks(new Vector3(x, y, z), 32);
        for (int i = 0; i < chunks.Length; i++)
        {
            yield return new WaitForEndOfFrame();
            chunks[i].Refresh();
            chunks[i].SetDirty();
        }
        yield return new WaitForEndOfFrame();
    }


    IEnumerator SetBurn(int x, int y, int z)
    {
        if (FireAudioSource == null)
        {
            FireAudioSource = Instantiate(Resources.Load("FireAreaSound"), new Vector3(x, y, z), Quaternion.identity) as GameObject;
        }
        yield return new WaitForSeconds(0f);
        //Global find block
        Block block = World.Instance.GetBlock(x, y, z);
        BlockSet.BlockSettings s;
        BlockSet.Blocks.TryGetValue(block.BlockType, out s);
        if (block != null && s.isFlamming)
        {
            burnedVoxels++;
            block.isBurns = true;
            World.Instance.SetBlock(x, y, z, block);

            GameObject Burn = Instantiate(Resources.Load("Burn"), new Vector3(x, y - 1, z), Quaternion.identity) as GameObject;
            Destroy(Burn, 1f);
            StartCoroutine(TimeDestroyBlock(x, y, z));
            CurrentChunk.SetDirty();
        }
    }


    IEnumerator TimeDestroyBlock(int x, int y, int z)  //Removing block
    {
        yield return new WaitForSeconds(2f);

        if (World.Instance.GetBlock(x, y, z).BlockType != BlockType.TNT)
        {
            World.Instance.SetBlock(x, y, z, 0);
        }
        else
        {
            Explode(x, y, z);
        }

        burnedVoxels--;
        isBurning = true;
    }

}