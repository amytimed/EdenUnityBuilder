using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preview of the selected block in UI
/// </summary>
public class BlockPreview : MonoBehaviour
{
    public MeshFilter PreviewMesh;
    public static Mesh MeshBlock;
    private Mesh _mesh;
    public int tiling;

    public GameObject FlowerIcon;
    public GameObject PreviewImage;
    void Start()
    {
        BlockSet.BlockSettings blockInfo;
        BlockSet.Blocks.TryGetValue(BlockType.Stone, out blockInfo);
        if (blockInfo != null)
        {
            UpdateIcon(blockInfo);
        }
    }

    public void UpdateIcon(BlockSet.BlockSettings blockInfo)
    {
        UpdatePreviewMesh(blockInfo);

        if (_mesh != null)
        {
            // UV
            List<Vector2> uv = new List<Vector2>();
            uv.AddRange(GetUVs(blockInfo.TexForward));
            uv.AddRange(GetUVs(blockInfo.TexForward));
            uv.AddRange(GetUVs(blockInfo.TexForward));
            uv.AddRange(GetUVs(blockInfo.TexForward));
            uv.AddRange(GetUVs(blockInfo.TexForward));
            uv.AddRange(GetUVs(blockInfo.TexForward));
            _mesh.uv = uv.ToArray();
            MeshBlock = _mesh;
        }
        else
        {
            Debug.LogError("Failed adding UV maps for preview");
        }
    }

    void UpdatePreviewMesh(BlockSet.BlockSettings blockInfo)
    {
        if (blockInfo.type == BlockType.Flowers)
        {
            PreviewMesh.transform.localEulerAngles = new Vector3(0, 0, 90);
            PreviewMesh.transform.localPosition = new Vector3(0.78f, -0.6f, -0.48f);
            FlowerIcon.SetActive(true);
            PreviewImage.SetActive(false);
        }
        else
        {
            FlowerIcon.SetActive(false);
            PreviewImage.SetActive(true);
            _mesh = new Mesh();

            _mesh.Clear();

            // TRIANGLES
            List<int> triangles = new List<int>();

            if (blockInfo.CustomBlock == 0)
            {
                PreviewMesh.transform.localEulerAngles = new Vector3(0, 0, 90);
                PreviewMesh.transform.localPosition = new Vector3(0.78f, -0.6f, -0.48f);
                // VERTICES
                _mesh.vertices = new Vector3[24]
                {
                //FORWARD FACE
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 0),

                //LEFT FACE
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),

                //BACK FACE
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),

                //RIGHT FACE
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1),
                new Vector3(1, 0, 0),

                //TOP FACE
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0),

                //BOTTOM FACE
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1)
                };
            }
            else if (blockInfo.CustomBlock == 2)
            {
                PreviewMesh.transform.localEulerAngles = new Vector3(0, 0, 0);
                PreviewMesh.transform.localPosition = new Vector3(-0.78f, -0.6f, -0.48f);
                _mesh.vertices = new Vector3[24]
                      {
                //FORWARD FACE
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),

                //LEFT FACE
                new Vector3(0, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),

                //BACK FACE
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),

                //RIGHT FACE
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1),
                new Vector3(1, 0, 0),

                //TOP FACE
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 0),

                //BOTTOM FACE
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1)
                      };

            }

            for (int i = 0; i < 6; i++)
            {
                int o = i * 4;
                triangles.AddRange(new int[]{
                    0+o, 1+o, 2+o,
                    2+o, 3+o, 0+o,
                    });
            }
            _mesh.triangles = triangles.ToArray();

            _mesh.RecalculateNormals();

            PreviewMesh.sharedMesh = _mesh;
        }
    }

    private Vector2[] GetUVs(float id)
    {
        List<Vector2> uv = new List<Vector2>();

        float tUnit = World.Instance.TerrainMaterialSettings.Tiling;
        uv.Add(new Vector2(tUnit * 1 + tUnit, tUnit * id));
        uv.Add(new Vector2(tUnit * 1 + tUnit, tUnit * id + tUnit));
        uv.Add(new Vector2(tUnit * 1, tUnit * id + tUnit));
        uv.Add(new Vector2(tUnit * 1, tUnit * id));

        return uv.ToArray();
    }
}
