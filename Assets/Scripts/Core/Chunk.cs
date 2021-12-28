using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
/// <summary>
/// Chunk class, block data
/// </summary>
public class Chunk : MonoBehaviour
{

    public Block[] Blocks; // 1D array blocks - ChunkSize*ChunkSize*ChunkSize

    public bool isDirty;

    public Vector3Int Position;

    private int _chunkSize = 16;

    private MeshFilter _meshFilter;

    private MeshCollider _meshCollider;

    private MeshRenderer _meshRenderer;

    public static readonly Vector3Int Dimensions = new Vector3Int(16, 16, 16);

    private World _world;

    private bool _isNeedsRefresh = false;

    private MeshData _meshData;

    public bool isLoaded = false;

    public bool isConverted = false;

    private Vector3 centeredPosition
    {
        get
        {
            return transform.position + (new Vector3(_chunkSize, _chunkSize, _chunkSize) / 2);
        }
    }

    private void Awake()
    {
        Position = Vector3Int.CeilToInt(transform.position);
    }

    void Start()
    {
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshCollider = gameObject.AddComponent<MeshCollider>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();

        _chunkSize = World.ChunkSize;
        gameObject.name = "Chunk_" + Position.x + "_" + Position.y + "_" + Position.z;
        Blocks = new Block[_chunkSize * _chunkSize * _chunkSize]; // init blocks
        _world = World.Instance;
        if (isLoaded == false)
        {
            Generate();
        }
    }

    /// <summary>
    /// Init chunk data
    /// </summary>

    public void InitData()
    {
        _world = World.Instance;
        Blocks = new Block[_chunkSize * _chunkSize * _chunkSize];
        for (int i = 0; i < Blocks.Length; i++)
        {
            Blocks[i] = new Block(BlockType.Air);
        }
    }

    /// <summary>
    /// Generates terrain for chunk
    /// </summary>
    public void Generate()
    {
        InitData();
        if (World.Instance.Type == WorldType.Flat) // IF WORLD SEED IS 0
        {
            if (Position.y == 32)
            {
                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int z = 0; z < _chunkSize; z++)
                    {
                        SetBlock(x, 0, z, new Block(BlockType.Grass));
                    }
                }
            }
            else if (Position.y == 16)
            {
                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int z = 0; z < _chunkSize; z++)
                    {
                        for (int y = 0; y < _chunkSize; y++)
                        {
                            SetBlock(x, y, z, new Block(BlockType.Dirt));
                        }
                    }
                }
            }
            else if (Position.y == 0)
            {
                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int z = 0; z < _chunkSize; z++)
                    {
                        for (int y = 0; y < _chunkSize; y++)
                        {
                            SetBlock(x, y, z, new Block(BlockType.Stone));
                        }
                        SetBlock(x, 0, z, new Block(BlockType.Bedrock));
                    }
                }
            }
        }
        else if (World.Instance.Type == WorldType.Normal)
        { // bad port of code terrain generation from original eden
          // ported from https://github.com/phonkee/EdenWorldBuilder/blob/master/Classes/TerrainGenerator.mm
            int TG_SEED = 400;
            //int TG_SEED2 = 123;
            int T_HEIGHT = (int)Position.y;
            int offsety = T_HEIGHT / 2 - 10;
            float var = 3f;
            const float NOISE_CONSTANT = 5.0f;
            bool genCaves = false;
            if (Position.y == 32)
            {
                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int z = 0; z < _chunkSize; z++)
                    {
                        int h;

                        float n = offsety;
                        float FREQ = 0.2f;
                        float AMPLITUDE = 4.0f;
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 vec = new Vector2((float)FREQ * (x + Position.x + TG_SEED) / NOISE_CONSTANT, (float)FREQ * (z + Position.z + TG_SEED) / NOISE_CONSTANT);
                            n += Perlin.Noise(vec.x, vec.y) * (AMPLITUDE) * var / 4;
                            FREQ *= 2;
                            AMPLITUDE /= 2;
                        }
                        h = Mathf.RoundToInt(n);
                        //  SetBlock(x, h, z, BlockType.Grass); // test

                        int FORMATION_HEIGHT = h - 6;
                        for (int y = 0; y < h; y++)
                        {
                            if (y < FORMATION_HEIGHT)
                            {
                                if (y > (h % 2 + 1) && y < FORMATION_HEIGHT - 16)
                                {
                                    if (!genCaves)
                                    {
                                        SetBlock(x, y, z, BlockType.Stone);
                                        continue;
                                    }
                                    float n3 = 0;
                                    float FREQ3 = 0.4f;
                                    float AMPLITUDE3 = 0.25f;
                                    for (int i = 0; i < 3; i++)
                                    {
                                        Vector3 vec = new Vector3(
                                   (float)FREQ3 * (x + Position.x + _world.WorldSeed) / NOISE_CONSTANT,
                                   (float)FREQ3 * (y + _world.WorldSeed) / NOISE_CONSTANT,
                                   (float)FREQ3 * (z + Position.z + _world.WorldSeed) / NOISE_CONSTANT);
                                        n3 += Perlin.Noise(vec) * (AMPLITUDE3);
                                        FREQ3 *= 2;
                                        AMPLITUDE3 /= 2;
                                    }

                                    if (n3 > 0)
                                    {
                                        if (n3 <= 0.01f)
                                            _world.SetBlock(x + Position.x, y + Position.y, z + Position.z, BlockType.DarkStone);
                                        else
                                            _world.SetBlock(x + Position.x, y + Position.y, z + Position.z, BlockType.Stone);

                                    }
                                    else
                                    {

                                        _world.SetBlock(x + Position.x, y + Position.y, z + Position.z, BlockType.Air);
                                    }
                                }
                                else
                                {
                                    _world.SetBlock(x + Position.x, y + Position.y, z + Position.z, BlockType.Stone);
                                }
                            }
                            else
                            {
                                float n3 = 0;
                                float FREQ3 = 0.3f;
                                float AMPLITUDE3 = 0.5f;
                                for (int i = 0; i < 3; i++)
                                {
                                    Vector3 vec = new Vector3(
                                    (float)FREQ3 * (x + Position.x + _world.WorldSeed) / NOISE_CONSTANT,
                                    (float)FREQ3 * (y + _world.WorldSeed) / NOISE_CONSTANT,
                                    (float)FREQ3 * (z + Position.z + _world.WorldSeed) / NOISE_CONSTANT);
                                    n3 += Perlin.Noise(vec) * (AMPLITUDE3);
                                    FREQ3 *= 2;
                                    AMPLITUDE3 /= 2;
                                }
                                if (n3 < 0.07f)
                                {
                                    SetBlock(x, y, z, BlockType.Dirt);
                                }
                            }
                        }
                        for (int y = 0; y < T_HEIGHT; y++)
                        {
                            if (GetBlock(x, y, z).BlockType == BlockType.Air)
                            {
                                if (GetBlock(x, y - 1, z).BlockType == BlockType.Dirt)
                                {
                                    if (UnityEngine.Random.value % 2 == 0 && (x + x % 10) % 20 < 4 && (z + z % 10) % 20 < 4)
                                    {
                                        //   setLandt(x, z, y, TYPE_FLOWER); // flower
                                        SetBlock(x, y, z, BlockType.Grass);
                                    }
                                    else if (UnityEngine.Random.value > 0.05f)
                                    {
                                        SetBlock(x, y, z, BlockType.Grass);
                                    }
                                    else
                                    {
                                        SetBlock(x, y, z, BlockType.Weeds);
                                    }
                                    //setColort(x ,z ,y-1 ,22+18);
                                    //setLandt(x ,z ,y-1 ,TYPE_GRASS);	
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                }

                for (int x = 2; x < _chunkSize - 2; x++)
                {
                    for (int y = 1; y < T_HEIGHT - 1; y++)
                    {
                        for (int z = 2; z < _chunkSize - 2; z++)
                        {
                            BlockType t = GetBlock(x, y, z).BlockType;
                            if (t == BlockType.Grass || t == BlockType.Weeds)
                            {
                                placeTree(x, y + 1, z, T_HEIGHT);
                            }
                        }
                    }
                }
            }
            else if (Position.y < 32 && Position.y >= 0)
            {
                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int y = 0; y < _chunkSize; y++)
                    {
                        for (int z = 0; z < _chunkSize; z++)
                        {
                            SetBlock(x, y, z, BlockType.Stone);
                            if (Position.y == 0)
                            {
                                SetBlock(x, 0, z, BlockType.Bedrock);
                            }
                        }
                    }
                }
            }
            int cloud = arc4random() % 5;
            if (cloud == 0 && Position.y == 64)
            {
                generateCloud(_chunkSize - 2);
            }
        }
        RefreshAsync();
    }

    int arc4random() //C# version of function used in C. not sure if there is a better solution
    {
        return UnityEngine.Random.Range(0, int.MaxValue);
    }

    /// <summary>
    /// Function from original Eden,
    /// generates simple square clouds
    /// </summary>
    void generateCloud(int T_HEIGHT)
    {
        int num = arc4random() % 4 + 4;
        for (int rep = 0; rep < num; rep++)
        {
            int x = arc4random() % 7;
            int y = arc4random() % 7;
            int w = arc4random() % (_chunkSize - x);
            int h = arc4random() % (_chunkSize - y);
            if (w < 4) w = 4;
            if (h < 4) h = 4;
            int d = arc4random() % 2 + 2;
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    SetBlock(i + x, T_HEIGHT - d, j + y, BlockType.Cloud);
                }
        }

    }
    /// <summary>
    /// Function from original Eden,
    /// generates trees
    /// </summary>
    void placeTree(int x, int z, int y, int T_HEIGHT)
    {
        //
        if (arc4random() % World.TREE_SPACING != 0) return;
        int tree_height = arc4random() % 3 + 6;
        if (y + tree_height >= T_HEIGHT) return;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = z - 1; j <= z + 1; j++)
            {
                BlockType t = GetBlock(i, y - 1, j).BlockType;
                if (!(t == BlockType.Grass || t == BlockType.Weeds || t == BlockType.Dirt))
                    return;
                if (GetBlock(i, y, j).BlockType != BlockType.Air) return;
            }
        }
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = z - 1; j <= z + 1; j++)
            {
                for (int k = y; k < y + tree_height; k++)
                {
                    BlockType t = GetBlock(i, k, j).BlockType;
                    if (t == BlockType.Air || t == BlockType.Leaves)
                        continue;
                    break;
                }
            }

        }

        for (int i = 0; i < 3 * tree_height / 4; i++)
        {
            SetBlock(x, y + i, z, BlockType.Trunk);
        }

        int color = arc4random() % 4;
        int[] ct = new int[4] { 0, 19, 20, 21 };
        BlockType type = BlockType.Leaves;

        for (int i = x - 2; i <= x + 2; i++)
        {
            for (int k = z - 2; k <= z + 2; k++)
            {
                for (int j = y + 2 * tree_height / 3; j < tree_height + y; j++)
                {
                    if (GetBlock(i, j, k).BlockType != BlockType.Leaves)
                    {
                        if (i == x - 2 || i == x + 2 || j == z - 2 || j == z + 2)
                        {
                            if ((i == x - 2 || i == x + 2) && (j == z - 2 || j == z + 2) && (k == y + 2 * tree_height / 3 || k == y + tree_height - 1))
                            {
                            }
                            else
                            if (arc4random() % 2 == 0)
                            {
                                SetBlock(i, j, k, type);
                                SetColor(i, j, k, (Paintings)ct[color]);
                            }
                        }
                        else
                        {
                            SetBlock(i, j, k, type);
                            SetColor(i, j, k, (Paintings)ct[color]);
                        }

                    }
                }
            }
        }

    }


    /// <summary>
    /// Refreshes the chunk asynchronously
    /// </summary>
    public void RefreshAsync()
    {
        Thread refreshThread = new Thread(new ThreadStart(Refresh));
        refreshThread.Start();
    }

    /// <summary>
    /// Refreshes the chunk synchronously.
    /// Blocks the main thread!
    /// </summary>

    public void Refresh()
    {
        _meshData = GenerateMesh(false);
        _isNeedsRefresh = true;
    }

    /// <summary>
    /// Applies changes to the mesh
    /// </summary>

    void BuildMesh()
    {
        if (_meshData != null)
        {
            _meshFilter.mesh.Clear();
            _meshFilter.mesh.subMeshCount = 2;
            _meshRenderer.materials[0] = _world.TerrainMaterialSettings.SolidMaterial;
            _meshRenderer.materials[1] = _world.TerrainMaterialSettings.TransparentMaterial;
            _meshFilter.mesh.vertices = _meshData.Vertices;
            // _meshFilter.mesh.triangles = _meshData.Triangles;
            _meshFilter.mesh.SetTriangles(_meshData.Triangles, 0);
            _meshFilter.mesh.SetTriangles(_meshData.TrianglesTransparent, 1);
            _meshFilter.mesh.colors32 = _meshData.Colors;
            _meshFilter.mesh.uv = _meshData.UVs;

            _meshFilter.mesh.RecalculateNormals();
            _meshFilter.mesh.Optimize();

            _meshCollider.sharedMesh = _meshFilter.mesh; // Collision. Maybe not use the unity physics for perfomance?
        }
    }

    /// <summary>
    /// If a chunk is marked with this, it will be stored in memory
    /// </summary>
    public void SetDirty()
    {
        isDirty = true;
    }

    void Update()
    {
        if (Vector3.Distance(_world.PlayerPosition, centeredPosition) >= _world.generationDistance + _chunkSize)
        {
            if (!isDirty)
            {
                _world.DestroyChunk(Position.x, Position.y, Position.z);
                gameObject.SetActive(false);
            }
            else
            {
                _meshRenderer.enabled = false;
            }
        }
        else
        {
            if (isDirty)
            {
                _meshRenderer.enabled = true;
            }
        }

        if (_isNeedsRefresh)
        {
            BuildMesh();
            _isNeedsRefresh = false;
        }
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        SetBlock(new Vector3Int(x, y, z), block);
    }

    /// <summary>
    /// Set the block locally
    /// </summary>

    public void SetBlock(int x, int y, int z, BlockType type)
    {
        SetBlock(new Vector3Int(x, y, z), new Block(type));
    }

    /// <summary>
    /// Get the block locally
    /// </summary>
    public Block GetBlock(int x, int y, int z)
    {
        return GetBlock(new Vector3Int(x, y, z));
    }


    /// <summary>
    /// Get the block globally
    /// </summary>
    public Block GetBlockG(int x, int y, int z) // Global get block
    {
        return _world.GetBlock(new Vector3(x + Position.x, y + Position.y, z + Position.z));
    }

    public void SetBlock(Vector3Int index, Block block)
    {
        if (!ContainsIndex(index))
        {
            return;
        }
        Blocks[FlattenIndex(index)] = block;
    }
    public void SetColor(int x, int y, int z, Paintings color)
    {
        Block block = GetBlock(x, y, z);
        block.Painting = color;
        SetBlock(x, y, z, block);
    }

    public Block GetBlock(Vector3Int index)
    {
        if (!ContainsIndex(index))
        {
            return new Block(BlockType.Air);
        }

        return Blocks[FlattenIndex(index)];
    }

    public bool ContainsIndex(Vector3Int index) =>
       index.x >= 0 && index.x < _chunkSize &&
       index.y >= 0 && index.y < _chunkSize &&
       index.z >= 0 && index.z < _chunkSize;

    private int FlattenIndex(Vector3Int index) =>
        (index.z * _chunkSize * _chunkSize) +
        (index.y * _chunkSize) +
        index.x;

    public bool CompareStep(Vector3Int a, Vector3Int b, int direction, bool backFace)
    {
        Block blockA = GetBlock(a);
        Block blockB = GetBlock(b);

        return blockB.IsSolid() && IsBlockFaceVisible(b, direction, backFace);
    }

    public bool IsBlockFaceVisible(Vector3Int blockPosition, int axis, bool backFace)
    {
        blockPosition[axis] += backFace ? -1 : 1;
        return !GetBlock(blockPosition).IsSolid();
    }

    /// <summary>
    /// Generates a mesh, if isGreedyMesh is true then will optimize the mesh (Broken)
    /// </summary>
    public MeshData GenerateMesh(bool isGreedyMesh)
    {
        if (isGreedyMesh)
        {
            MeshBuilder builder = new MeshBuilder();
            bool[,] merged;

            Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
            Vector3[] vertices;

            Block startBlock;
            int direction, workAxis1, workAxis2;

            // Iterate over each face of the blocks.
            for (int face = 0; face < 6; face++)
            {
                bool isBackFace = face > 2;
                direction = face % 3;
                workAxis1 = (direction + 1) % 3;
                workAxis2 = (direction + 2) % 3;

                startPos = new Vector3Int();
                currPos = new Vector3Int();

                // Iterate over the chunk layer by layer.
                for (startPos[direction] = 0; startPos[direction] < Dimensions[direction]; startPos[direction]++)
                {
                    merged = new bool[Dimensions[workAxis1], Dimensions[workAxis2]];

                    // Build the slices of the mesh.
                    for (startPos[workAxis1] = 0; startPos[workAxis1] < Dimensions[workAxis1]; startPos[workAxis1]++)
                    {
                        for (startPos[workAxis2] = 0; startPos[workAxis2] < Dimensions[workAxis2]; startPos[workAxis2]++)
                        {
                            startBlock = GetBlock(startPos);

                            // If this block has already been merged, is air, or not visible skip it.
                            if (merged[startPos[workAxis1], startPos[workAxis2]] || !startBlock.IsSolid() || !IsBlockFaceVisible(startPos, direction, isBackFace))
                            {
                                continue;
                            }

                            // Reset the work var
                            quadSize = new Vector3Int();

                            // Figure out the width, then save it
                            for (currPos = startPos, currPos[workAxis2]++; currPos[workAxis2] < Dimensions[workAxis2] && CompareStep(startPos, currPos, direction, isBackFace) && !merged[currPos[workAxis1], currPos[workAxis2]]; currPos[workAxis2]++) { }
                            quadSize[workAxis2] = currPos[workAxis2] - startPos[workAxis2];

                            // Figure out the height, then save it
                            for (currPos = startPos, currPos[workAxis1]++; currPos[workAxis1] < Dimensions[workAxis1] && CompareStep(startPos, currPos, direction, isBackFace) && !merged[currPos[workAxis1], currPos[workAxis2]]; currPos[workAxis1]++)
                            {
                                for (currPos[workAxis2] = startPos[workAxis2]; currPos[workAxis2] < Dimensions[workAxis2] && CompareStep(startPos, currPos, direction, isBackFace) && !merged[currPos[workAxis1], currPos[workAxis2]]; currPos[workAxis2]++) { }

                                // If we didn't reach the end then its not a good add.
                                if (currPos[workAxis2] - startPos[workAxis2] < quadSize[workAxis2])
                                {
                                    break;
                                }
                                else
                                {
                                    currPos[workAxis2] = startPos[workAxis2];
                                }
                            }
                            quadSize[workAxis1] = currPos[workAxis1] - startPos[workAxis1];

                            // Now we add the quad to the mesh
                            m = new Vector3Int();
                            m[workAxis1] = quadSize[workAxis1];

                            n = new Vector3Int();
                            n[workAxis2] = quadSize[workAxis2];

                            // We need to add a slight offset when working with front faces.
                            offsetPos = startPos;
                            offsetPos[direction] += isBackFace ? 0 : 1;

                            //Draw the face to the mesh
                            vertices = new Vector3[] {
                            offsetPos,
                            offsetPos + m,
                            offsetPos + m + n,
                            offsetPos + n
                        };

                            builder.AddSquareFace(vertices, startBlock.GetColor(), isBackFace);

                            // Mark it merged
                            for (int f = 0; f < quadSize[workAxis1]; f++)
                            {
                                for (int g = 0; g < quadSize[workAxis2]; g++)
                                {
                                    merged[startPos[workAxis1] + f, startPos[workAxis2] + g] = true;
                                }
                            }
                        }
                    }
                }
            }
            return builder.ToMeshData();
        } // Greedy meshing method (UVs is broken)
        else // Simple method
        {
            MeshBuilder builder = new MeshBuilder();
            for (int x = 0; x < _chunkSize; x++)
            {
                for (int y = 0; y < _chunkSize; y++)
                {
                    for (int z = 0; z < _chunkSize; z++)
                    {
                        BlockType blocktype = BlockType.Air;
                        if (GetBlock(x, y, z) != null)
                        {
                            blocktype = GetBlock(x, y, z).BlockType;
                        }
                        if (blocktype != BlockType.Air) // later add check blocks in neighbor chunks
                        {
                            BlockSet.BlockSettings blocksettings;
                            BlockSet.Blocks.TryGetValue(blocktype, out blocksettings);
                            if (blocksettings == null)
                            {
                                BlockSet.Blocks.TryGetValue(BlockType.Dirt, out blocksettings);
                            }
                            Vector2 texUp = Vector2.zero;
                            Vector2 texDown = Vector2.zero;
                            Vector2 texForward = Vector2.zero;
                            Vector2 texBack = Vector2.zero;
                            Vector2 texRight = Vector2.zero;
                            Vector2 texLeft = Vector2.zero;
                            bool isTransparent = false;
                            bool isAnimated = false;

                            if (blocksettings != null)
                            {
                                texUp = new Vector2(0, blocksettings.TexUp);
                                texDown = new Vector2(0, blocksettings.TexDown);
                                texForward = new Vector2(0, blocksettings.TexForward);
                                texBack = new Vector2(0, blocksettings.TexBack);
                                texRight = new Vector2(0, blocksettings.TexRight);
                                texLeft = new Vector2(0, blocksettings.TexLeft);
                                isTransparent = blocksettings.isTransparent;
                                isAnimated = false; // later add
                            }

                            Color colorBlock = GetBlock(x, y, z).GetColor();

                            if (!GetBlockG(x + 1, y, z).IsSolid() || (isTransparent == false && GetBlockG(x + 1, y, z).IsTransparent()))
                            {
                                builder.AddSimpleFace(x, y, z, 1, BlockDirection.Right, colorBlock, texRight, isTransparent);
                            }
                            if (!GetBlockG(x - 1, y, z).IsSolid() || (isTransparent == false && GetBlockG(x - 1, y, z).IsTransparent()))
                            {
                                builder.AddSimpleFace(x, y, z, 1, BlockDirection.Left, colorBlock, texLeft, isTransparent);
                            }
                            if (!GetBlockG(x, y + 1, z).IsSolid() || (isTransparent == false && GetBlockG(x, y + 1, z).IsTransparent()))
                            {
                                builder.AddSimpleFace(x, y, z, 1, BlockDirection.Up, colorBlock, texUp, isTransparent);
                            }
                            if (!GetBlockG(x, y - 1, z).IsSolid() || (isTransparent == false && GetBlockG(x, y - 1, z).IsTransparent()))
                            {
                                builder.AddSimpleFace(x, y, z, 1, BlockDirection.Down, colorBlock, texDown, isTransparent);
                            }
                            if (!GetBlockG(x, y, z + 1).IsSolid() || (isTransparent == false && GetBlockG(x, y, z + 1).IsTransparent()))
                            {
                                builder.AddSimpleFace(x, y, z, 1, BlockDirection.Forward, colorBlock, texForward, isTransparent);
                            }
                            if (!GetBlockG(x, y, z - 1).IsSolid() || (isTransparent == false && GetBlockG(x, y, z - 1).IsTransparent()))
                            {
                                builder.AddSimpleFace(x, y, z, 1, BlockDirection.Back, colorBlock, texBack, isTransparent);
                            }
                        }
                    }
                }

            }
            return builder.ToMeshData();
        }
    }
}
