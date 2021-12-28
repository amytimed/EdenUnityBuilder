using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    private readonly List<Vector3> vertices;
    private readonly List<int> triangles;
    private readonly List<int> trianglesTransparent;
    private readonly List<Color32> colors;
    private readonly List<Vector2> uvs;

    private int faceCount = 0;
    private int faceCountTransparent = 0;

    public MeshBuilder()
    {
        vertices = new List<Vector3>();

        triangles = new List<int>();
        trianglesTransparent = new List<int>();

        colors = new List<Color32>();
        uvs = new List<Vector2>();
    }

    public void AddSquareFace(Vector3[] vertices, Color32 color, bool isBackFace) // test
    {
        if (vertices.Length != 4)
        {
            throw new ArgumentException("A square face requires 4 vertices");
        }

        // Add the 4 vertices, and color for each vertex.
        for (int i = 0; i < vertices.Length; i++)
        {
            this.vertices.Add(vertices[i]);
            colors.Add(color);
            uvs.Add(Vector2.zero); // test uvs
        }

        if (!isBackFace)
        {
            triangles.Add(this.vertices.Count - 4);
            triangles.Add(this.vertices.Count - 3);
            triangles.Add(this.vertices.Count - 2);

            triangles.Add(this.vertices.Count - 4);
            triangles.Add(this.vertices.Count - 2);
            triangles.Add(this.vertices.Count - 1);
        }
        else
        {
            triangles.Add(this.vertices.Count - 2);
            triangles.Add(this.vertices.Count - 3);
            triangles.Add(this.vertices.Count - 4);

            triangles.Add(this.vertices.Count - 1);
            triangles.Add(this.vertices.Count - 2);
            triangles.Add(this.vertices.Count - 4);
        }
    }

    public void AddSimpleFace(int x, int y, int z, byte id, BlockDirection dir, Color color, Vector2 texturePos, bool isTransparent)
    {
        switch (dir)
        {
            case BlockDirection.Up:
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x, y, z));
                //Colors
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                Cube(texturePos, isTransparent);
                break;
            case BlockDirection.Forward:
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y - 1, z + 1));
                //Colors
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                Cube(texturePos, isTransparent);
                break;
            case BlockDirection.Right:
                vertices.Add(new Vector3(x + 1, y - 1, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                //Colors
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                Cube(texturePos, isTransparent);
                break;
            case BlockDirection.Back:
                vertices.Add(new Vector3(x, y - 1, z));
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y - 1, z));
                //Colors
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                Cube(texturePos, isTransparent);
                break;
            case BlockDirection.Left:
                vertices.Add(new Vector3(x, y - 1, z + 1));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y - 1, z));
                //Colors
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                Cube(texturePos, isTransparent);
                break;
            case BlockDirection.Down:
                vertices.Add(new Vector3(x, y - 1, z));
                vertices.Add(new Vector3(x + 1, y - 1, z));
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                vertices.Add(new Vector3(x, y - 1, z + 1));
                //Colors
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                Cube(texturePos, isTransparent);
                break;
            default:
                Debug.Log("idk");
                break;
        }
    }


    void Cube(Vector2 texturePos, bool isTransparent)
    {
        if (isTransparent)
        {
            trianglesTransparent.Add(faceCountTransparent * 4); //1
            trianglesTransparent.Add(faceCountTransparent * 4 + 1); //2
            trianglesTransparent.Add(faceCountTransparent * 4 + 2); //3
            trianglesTransparent.Add(faceCountTransparent * 4); //1
            trianglesTransparent.Add(faceCountTransparent * 4 + 2); //3
            trianglesTransparent.Add(faceCountTransparent * 4 + 3); //4
        }
        else
        {
            triangles.Add(faceCount * 4); //1
            triangles.Add(faceCount * 4 + 1); //2
            triangles.Add(faceCount * 4 + 2); //3
            triangles.Add(faceCount * 4); //1
            triangles.Add(faceCount * 4 + 2); //3
            triangles.Add(faceCount * 4 + 3); //4
        }
        float tUnit = World.Instance.TerrainMaterialSettings.Tiling;
        uvs.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        uvs.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        uvs.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        uvs.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++;
        faceCountTransparent++;
    }

    public MeshData ToMeshData()
    {
        MeshData data = new MeshData(
            vertices.ToArray(),
            triangles.ToArray(),
            trianglesTransparent.ToArray(),
            colors.ToArray(),
            uvs.ToArray()
        );

        vertices.Clear();
        triangles.Clear();
        trianglesTransparent.Clear();
        colors.Clear();
        uvs.Clear();

        return data;
    }

    private Vector2[] GetUVs(int id)
    {
        Vector2[] uv = new Vector2[4];
        float tiling = World.Instance.TerrainMaterialSettings.Tiling;

        int id2 = id + 1;
        float o = 1f / tiling;
        int i = 0;

        for (int y = 0; y < tiling; y++)
        {
            for (int x = 0; x < tiling; x++)
            {
                i++;

                if (i == id2)
                {
                    float padding = World.Instance.TerrainMaterialSettings.UVPadding / tiling; // Adding a little padding to prevent UV bleeding (to fix)
                    uv[0] = new Vector2(x / tiling + padding, 1f - (y / tiling) - padding);
                    uv[1] = new Vector2(x / tiling + o - padding, 1f - (y / tiling) - padding);
                    uv[2] = new Vector2(x / tiling + o - padding, 1f - (y / tiling + o) + padding);
                    uv[3] = new Vector2(x / tiling + padding, 1f - (y / tiling + o) + padding);

                    return uv;
                }
            }
        }

        uv[0] = Vector2.zero;
        uv[1] = Vector2.zero;
        uv[2] = Vector2.zero;
        uv[3] = Vector2.zero;

        return uv;
    }
}