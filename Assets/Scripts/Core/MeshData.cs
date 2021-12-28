using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public Vector3[] Vertices { get; }
    public int[] Triangles { get; }

    public int[] TrianglesTransparent { get; }
    public Color32[] Colors { get; }
    public Vector2[] UVs { get; }

    public MeshData(Vector3[] vertices, int[] triangles, int[] trianglesTransparent, Color32[] colors, Vector2[] uvs)
    {
        Vertices = vertices;
        Triangles = triangles;
        Colors = colors;
        UVs = uvs;
        TrianglesTransparent = trianglesTransparent;
    }
}